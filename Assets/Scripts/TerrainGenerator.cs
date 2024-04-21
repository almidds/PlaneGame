using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    ComputeBuffer _weightsBuffer;

    [SerializeField]
    private ComputeShader terrainGenerator;

    void CreateBuffers() {
        _weightsBuffer = new ComputeBuffer (
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk,
            sizeof(float)
        );
    }

    void ReleaseBuffers() {
        _weightsBuffer.Release();
    }

    public float[] GetTerrain() {
        CreateBuffers();
        float[] noiseValues = new float[TerrainMetrics.PointsPerChunk * 
                                        TerrainMetrics.PointsPerChunk *
                                        TerrainMetrics.PointsPerChunk];

        terrainGenerator.SetBuffer(0, "_Weights", _weightsBuffer);

        terrainGenerator.Dispatch(0,
                                  TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                  TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                  TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads);

        _weightsBuffer.GetData(noiseValues);
        ReleaseBuffers();
        return noiseValues;
    }
}
