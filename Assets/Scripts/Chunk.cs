using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chunk : MonoBehaviour
{

    float[] _weights;

    public MeshFilter meshFilter;

    public bool autoUpdate;

    [Header("Noise")]
    public int seed;
    public int numOctaves = 4;
    public float noiseScale = 1;
    public float lacunarity = 2;
    public float gain = 0.5f;
    public float floorOffset = 0;
    public float hardFloor = 0;
    public float hardFloorWeight = 0;
    public float weightMultiplier = 1;
    public float noiseWeight = 1;
    public Vector4 shaderParams;


    public void GenerateMap()
    {
        NoiseGenerator terrainGenerator = gameObject.GetComponent<NoiseGenerator>();
        _weights = terrainGenerator.GenerateNoise(transform.position, seed, numOctaves, noiseScale, lacunarity, gain, floorOffset, hardFloor, hardFloorWeight, weightMultiplier, noiseWeight, shaderParams);

        MeshGenerator meshGenerator = gameObject.GetComponent<MeshGenerator>();
        meshFilter.sharedMesh = meshGenerator.ConstructMesh(_weights);
    }
}