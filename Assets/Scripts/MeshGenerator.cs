using System.Buffers;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public ComputeShader meshGenerator;

    protected List<ComputeBuffer> buffersToRelease;
    protected List<ComputeBuffer> buffersToCreate;


    struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public static int SizeOf = sizeof(float) * 3 * 3;
    }

    void ReleaseBuffers()
    {
        if (buffersToRelease != null)
        {
            foreach (var buffer in buffersToRelease)
            {
                buffer.Release();
            }
        }
    }

    ComputeBuffer trianglesBuffer;
    ComputeBuffer trianglesCountBuffer;
    ComputeBuffer pointsBuffer;

    int ReadTriangleCount()
    {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(trianglesBuffer, trianglesCountBuffer, 0);
        trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    public Mesh ConstructMesh(float[] points)
    {
        trianglesBuffer = new ComputeBuffer(5 * TerrainMetrics.PointsPerChunk
                                              * TerrainMetrics.PointsPerChunk
                                              * TerrainMetrics.PointsPerChunk,
                                            Triangle.SizeOf,
                                            ComputeBufferType.Append);
        trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        pointsBuffer = new ComputeBuffer(TerrainMetrics.PointsPerChunk
                                         * TerrainMetrics.PointsPerChunk
                                         * TerrainMetrics.PointsPerChunk,
                                         sizeof(float));
        buffersToRelease = new List<ComputeBuffer> {trianglesBuffer,
                                                    trianglesCountBuffer,
                                                    pointsBuffer};

        meshGenerator.SetBuffer(0, "triangles", trianglesBuffer);
        meshGenerator.SetBuffer(0, "points", pointsBuffer);

        meshGenerator.SetInt("numPointsPerAxis", TerrainMetrics.PointsPerChunk);
        meshGenerator.SetFloat("chunkSize", TerrainMetrics.ChunkSize);
        meshGenerator.SetFloat("isoLevel", -0.1f);

        pointsBuffer.SetData(points);
        trianglesBuffer.SetCounterValue(0);
        meshGenerator.Dispatch(0, TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                    TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                    TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads);
        Triangle[] tris = new Triangle[ReadTriangleCount()];
        trianglesBuffer.GetData(tris);

        ReleaseBuffers();

        return CreateMeshFromTriangles(tris);
    }

    Mesh CreateMeshFromTriangles(Triangle[] triangles)
    {
        Vector3[] verts = new Vector3[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++)
        {
            int startIndex = i * 3;
            verts[startIndex] = triangles[i].a;
            verts[startIndex + 1] = triangles[i].b;
            verts[startIndex + 2] = triangles[i].c;
            tris[startIndex] = startIndex;
            tris[startIndex + 1] = startIndex + 1;
            tris[startIndex + 2] = startIndex + 2;
        }

        Mesh mesh = new Mesh
        {
            vertices = verts,
            triangles = tris
        };
        mesh.RecalculateNormals();
        return mesh;
    }

}