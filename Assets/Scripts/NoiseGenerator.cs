using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{

    ComputeBuffer _weightsBuffer;

    [SerializeField]
    private ComputeShader noiseGenerator;

    [Header ("Noise")]
    public int seed;
    public int numOctaves = 4;
    public float noiseScale = 1;
    public float lacunarity = 2;
    public float gain = 0.5f;
    public float amplitude = 1;


    public float[] GenerateNoise(Vector3 position) {
        _weightsBuffer = new ComputeBuffer (
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk *
            TerrainMetrics.PointsPerChunk,
            sizeof(float)
        );
        noiseGenerator.SetBuffer(0, "_weights", _weightsBuffer);

        noiseGenerator.SetInt("numPointsPerAxis", TerrainMetrics.PointsPerChunk);
        noiseGenerator.SetFloat("chunkSize", TerrainMetrics.ChunkSize);
        noiseGenerator.SetVector("chunkCentre", position);

        noiseGenerator.SetInt("seed", seed);
        noiseGenerator.SetInt("octaves", numOctaves);
        noiseGenerator.SetFloat("noiseScale", noiseScale);
        noiseGenerator.SetFloat("lacunarity", lacunarity);
        noiseGenerator.SetFloat("gain", gain);
        noiseGenerator.SetFloat("amplitude", amplitude);


        noiseGenerator.Dispatch(0,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads,
                                TerrainMetrics.PointsPerChunk / TerrainMetrics.NumThreads);
        float[] noiseValues = new float[TerrainMetrics.PointsPerChunk * 
                                        TerrainMetrics.PointsPerChunk *
                                        TerrainMetrics.PointsPerChunk];
        _weightsBuffer.GetData(noiseValues);
        _weightsBuffer.Release();
        return noiseValues;
    }
}
