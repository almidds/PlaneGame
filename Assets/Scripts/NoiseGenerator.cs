using System.Buffers;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{

    ComputeBuffer _weightsBuffer;

    public ComputeShader noiseGenerator;


    public float[] GenerateNoise(Vector3 position, int seed, int numOctaves, float noiseScale, float lacunarity, float gain, float floorOffset, float hardFloor, float hardFloorWeight, float weightMultiplier, float noiseWeight, Vector4 shaderParams)
    {
        var prng = new System.Random(seed);
        var offsets = new Vector3[numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < numOctaves; i++)
        {
            offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
        }
        var offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
        offsetsBuffer.SetData(offsets);

        _weightsBuffer = new ComputeBuffer(
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk,
            sizeof(float)
        );
        noiseGenerator.SetBuffer(0, "_weights", _weightsBuffer);
        noiseGenerator.SetBuffer(0, "offsets", offsetsBuffer);

        noiseGenerator.SetInt("numPointsPerAxis", TerrainMetrics.PointsPerChunk);
        noiseGenerator.SetFloat("chunkSize", TerrainMetrics.ChunkSize);
        noiseGenerator.SetVector("chunkCentre", position);

        noiseGenerator.SetInt("numOctaves", numOctaves);
        noiseGenerator.SetFloat("noiseScale", noiseScale);
        noiseGenerator.SetFloat("lacunarity", lacunarity);
        noiseGenerator.SetFloat("gain", gain);
        noiseGenerator.SetFloat("weightMultiplier", weightMultiplier);
        noiseGenerator.SetFloat("noiseWeight", noiseWeight);
        noiseGenerator.SetVector("params", shaderParams);

        noiseGenerator.SetFloat("floorOffset", floorOffset);
        noiseGenerator.SetFloat("hardFloor", hardFloor);
        noiseGenerator.SetFloat("hardFloorWeight", hardFloorWeight);


        noiseGenerator.Dispatch(0,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads);
        float[] noiseValues = new float[TerrainMetrics.PointsPerChunk *
                                        TerrainMetrics.PointsPerChunk *
                                        TerrainMetrics.PointsPerChunk];
        _weightsBuffer.GetData(noiseValues);
        _weightsBuffer.Release();
        offsetsBuffer.Release();

        return noiseValues;
    }
}