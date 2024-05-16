using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMetrics {
    public const float ChunkSize = 16;
    public const int PointsPerChunk = 16;
    public const float PointSpacing = ChunkSize / (PointsPerChunk - 1);
    public const int NumThreads = 8;
}
