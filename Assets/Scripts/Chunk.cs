using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chunk : MonoBehaviour
{

    float[] _weights;

    public MeshFilter meshFilter;

    public void GenerateMap()
    {
        NoiseGenerator terrainGenerator = gameObject.GetComponent<NoiseGenerator>();
        _weights = terrainGenerator.GenerateNoise(transform.position);

        MeshGenerator meshGenerator = gameObject.GetComponent<MeshGenerator>();
        meshFilter.sharedMesh = meshGenerator.ConstructMesh(_weights);
    }
}