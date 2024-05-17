using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ChunkController : MonoBehaviour
{

    public int numChunks = 4;
    public GameObject chunkPrefab;
    GameObject[] chunksToRemove;


    public void GenerateChunks() {
        CleanUpChunks();
        chunksToRemove = new GameObject[numChunks * numChunks * numChunks];
        for (int x = 0; x < numChunks; x++) {
            for (int y = 0; y < numChunks; y++) {
                for (int z = 0; z < numChunks; z++) {
                    GameObject chunk = Instantiate(chunkPrefab);
                    chunksToRemove[x + numChunks * (y + numChunks * z)] = chunk;
                    chunk.transform.position = new Vector3(x * TerrainMetrics.ChunkSize, y * TerrainMetrics.ChunkSize, z * TerrainMetrics.ChunkSize);
                    chunk.AddComponent<Chunk>();
                    chunk.GetComponent<Chunk>().GenerateMap();
                    
                }
            }
        }
    }

    void CleanUpChunks() {
        if (chunksToRemove == null) {
            return;
        }
        foreach (GameObject chunk in chunksToRemove) {
            DestroyImmediate(chunk);
        }
    }
}
