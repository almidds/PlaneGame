using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    float[] _weights;
    
    public void GenerateMap() {
        NoiseGenerator terrainGenerator = gameObject.GetComponent<NoiseGenerator>();
        _weights = terrainGenerator.GenerateNoise(transform.position);
    }

    private void OnDrawGizmos() {
        if (_weights == null || _weights.Length == 0) {
            return;
        }
        for (int x = 0; x < TerrainMetrics.PointsPerChunk; x++) {
            for (int y = 0; y < TerrainMetrics.PointsPerChunk; y++) {
                for (int z = 0; z < TerrainMetrics.PointsPerChunk; z++) {
                    int index = x + TerrainMetrics.PointsPerChunk * (y + TerrainMetrics.PointsPerChunk * z);
                    float noiseValue = _weights[index];
                    Gizmos.color = Color.Lerp(Color.black, Color.white, noiseValue);
                    Gizmos.DrawCube(new Vector3(transform.position.x + TerrainMetrics.PointSpacing * x, transform.position.y + TerrainMetrics.PointSpacing * y, transform.position.z +  TerrainMetrics.PointSpacing * z), Vector3.one * .2f);
                }
            }
        }
    }
}
