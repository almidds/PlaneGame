static const uint numThreads = 8;

// Number of points in each direction for a single chunk
int numPointsPerAxis;

// Size of a single chunk
float chunkSize;
float isoLevel;
float3 offset;

float pointSpacing() {
    return chunkSize / (float) (numPointsPerAxis - 1);
}

int indexFromCoord(int x, int y, int z) {
    return x + numPointsPerAxis * (y + numPointsPerAxis * z);
}

// Chunk specific parameters
// Centre of the chunk
float3 chunkCentre;
