using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

using Unity.VisualScripting;

using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [Header("Player")]
    public Transform viewer;
    public float viewDistance;

    private List<Chunk> _chunks;
    private Dictionary<Vector3Int, Chunk> _existingChunks;
    private Queue<Chunk> _recyclableChunks;

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

    [Header("Chunk Objects")]
    public Material terrainMat;
    public ComputeShader noiseShader;
    public ComputeShader meshShader;

    GameObject _chunkHolder;
    const string CHUNKHOLDERNAME = "Chunk Holder";

    void Awake()
    {
        if (Application.isPlaying)
        {
            InitVariableChunkStructures();

            var oldChunks = FindObjectsOfType<Chunk>();
            foreach (Chunk oldChunk in oldChunks)
            {
                Destroy(oldChunk.gameObject);
            }
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            Run();
        }
    }

    void InitVariableChunkStructures()
    {
        _recyclableChunks = new Queue<Chunk>();
        _chunks = new List<Chunk>();
        _existingChunks = new Dictionary<Vector3Int, Chunk>();
    }

    void Run()
    {
        InitVisibleChunks();
    }

    void InitVisibleChunks()
    {
        if (_chunks == null)
        {
            return;
        }
        CreateChunkHolder();

        Vector3 viewerPosition = viewer.position;
        Vector3 normalisedViewerPosition = viewerPosition / TerrainMetrics.ChunkSize;
        Vector3Int viewerCoord = new Vector3Int(Mathf.RoundToInt(normalisedViewerPosition.x),
                                                Mathf.RoundToInt(normalisedViewerPosition.y),
                                                Mathf.RoundToInt(normalisedViewerPosition.z));

        int maxChunksInView = Mathf.CeilToInt(viewDistance / TerrainMetrics.ChunkSize);
        float sqrViewDistance = viewDistance * viewDistance;

        // Loop through all chunks and flag for deletion if outside of max view distance
        for (int i = _chunks.Count - 1; i >= 0; i--)
        {
            Chunk chunk = _chunks[i];
            float sqrDst = GetSqrDst(viewerPosition, chunk.coord);
            if (sqrDst > sqrViewDistance)
            {
                _existingChunks.Remove(chunk.coord);
                _recyclableChunks.Enqueue(chunk);
                _chunks.RemoveAt(i);
            }
        }

        // Loop through all possible chunks and create if they don't exist
        for (int x = -maxChunksInView; x <= maxChunksInView; x++)
        {
            for (int y = -maxChunksInView; y <= maxChunksInView; y++)
            {
                for (int z = -maxChunksInView; z <= maxChunksInView; z++)
                {
                    Vector3Int coord = new Vector3Int(x, y, z) + viewerCoord;

                    if (_existingChunks.ContainsKey(coord))
                    {
                        continue;
                    }

                    float sqrDst = GetSqrDst(viewerPosition, coord);

                    if (sqrDst <= sqrViewDistance)
                    {
                        Bounds bounds = new Bounds(CentreFromCoord(coord),
                                                   Vector3.one * TerrainMetrics.ChunkSize);
                        // if (IsVisibleFrom(bounds, Camera.main))
                        // {
                        if (_recyclableChunks.Count > 0)
                        {
                            Chunk chunk = _recyclableChunks.Dequeue();
                            chunk.coord = coord;
                            _existingChunks.Add(coord, chunk);
                            _chunks.Add(chunk);
                            UpdateChunkMesh(chunk);
                        }
                        else
                        {
                            Chunk chunk = CreateChunk(coord);
                            chunk.coord = coord;
                            chunk.SetUp(terrainMat, seed, numOctaves, noiseScale,
                                        lacunarity, gain, floorOffset, hardFloor,
                                        hardFloorWeight, weightMultiplier, noiseWeight,
                                        shaderParams);
                            _existingChunks.Add(coord, chunk);
                            _chunks.Add(chunk);
                            UpdateChunkMesh(chunk);
                        }
                        // }
                    }
                }
            }
        }
    }

    float GetSqrDst(Vector3 viewerPosition, Vector3Int coord)
    {
        Vector3 centre = CentreFromCoord(coord);
        Vector3 viewerOffset = viewerPosition - centre;
        Vector3 o = new Vector3(Mathf.Abs(viewerOffset.x),
                                Mathf.Abs(viewerOffset.y),
                                Mathf.Abs(viewerOffset.z))
                                - Vector3.one * TerrainMetrics.ChunkSize / 2;
        float sqrDst = new Vector3(Mathf.Max(o.x, 0),
                                   Mathf.Max(o.y, 0),
                                   Mathf.Max(o.z, 0)).sqrMagnitude;
        return sqrDst;
    }

    bool IsVisibleFrom(Bounds bounds, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    void UpdateChunkMesh(Chunk chunk)
    {
        chunk.GenerateMap();
    }

    Chunk CreateChunk(Vector3Int coord)
    {
        GameObject chunk = new GameObject($"Chunk ({coord.x}, {coord.y}, {coord.z})");
        chunk.transform.parent = _chunkHolder.transform;
        Chunk newChunk = chunk.AddComponent<Chunk>();
        NoiseGenerator ng = newChunk.AddComponent<NoiseGenerator>();
        ng.noiseGenerator = noiseShader;
        MeshGenerator mg = newChunk.AddComponent<MeshGenerator>();
        mg.meshGenerator = meshShader;
        newChunk.coord = coord;
        return newChunk;
    }

    void CreateChunkHolder()
    {
        if (_chunkHolder == null)
        {
            if (GameObject.Find(CHUNKHOLDERNAME))
            {
                _chunkHolder = GameObject.Find(CHUNKHOLDERNAME);
            }
            else
            {
                _chunkHolder = new GameObject(CHUNKHOLDERNAME);
            }
        }
    }

    Vector3 CentreFromCoord(Vector3Int coord)
    {
        return new Vector3(coord.x, coord.y, coord.z) * TerrainMetrics.ChunkSize;
    }
}