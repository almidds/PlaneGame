using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chunk : MonoBehaviour
{

    public Vector3Int coord;

    float[] _weights;

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


    [Header("Mesh")]
    public Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public void GenerateMap()
    {
        transform.position = CentreFromCoord(coord);
        NoiseGenerator terrainGenerator = gameObject.GetComponent<NoiseGenerator>();
        _weights = terrainGenerator.GenerateNoise(transform.position, seed, numOctaves, noiseScale, lacunarity, gain, floorOffset, hardFloor, hardFloorWeight, weightMultiplier, noiseWeight, shaderParams);

        MeshGenerator meshGenerator = gameObject.GetComponent<MeshGenerator>();
        meshFilter.sharedMesh = meshGenerator.ConstructMesh(_weights);
    }

    Vector3 CentreFromCoord(Vector3Int coord)
    {
        return new Vector3(coord.x, coord.y, coord.z) * TerrainMetrics.ChunkSize;
    }

    public void SetUp(Material terrainMat, int seed, int numOctaves, float noiseScale, float lacunarity, float gain, float floorOffset, float hardFloor, float hardFloorWeight, float weightMultiplier, float noiseWeight, Vector4 shaderParams)
    {
        this.seed = seed;
        this.numOctaves = numOctaves;
        this.noiseScale = noiseScale;
        this.lacunarity = lacunarity;
        this.gain = gain;
        this.floorOffset = floorOffset;
        this.hardFloor = hardFloor;
        this.hardFloorWeight = hardFloorWeight;
        this.weightMultiplier = weightMultiplier;
        this.noiseWeight = noiseWeight;
        this.shaderParams = shaderParams;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            meshFilter.sharedMesh = mesh;
        }

        if (meshCollider.sharedMesh == null)
        {
            meshCollider.sharedMesh = mesh;
        }
        meshCollider.enabled = false;
        meshCollider.enabled = true;
        meshRenderer.material = terrainMat;
    }

}