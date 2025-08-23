using UnityEngine;

public class Overworld
{
    public readonly int width, height;           // world tiles
    public readonly int seed;
    public readonly int chunksX, chunksY;
    public readonly WorldChunk[,] chunks;

    public Overworld(int width, int height, int seed)
    {
        this.width = width; this.height = height; this.seed = seed;
        chunksX = Mathf.CeilToInt(width  / (float)WorldChunk.Size);
        chunksY = Mathf.CeilToInt(height / (float)WorldChunk.Size);
        chunks = new WorldChunk[chunksX, chunksY];
        for (int y = 0; y < chunksY; y++)
            for (int x = 0; x < chunksX; x++)
                chunks[x, y] = new WorldChunk();
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    public ref WorldTile Get(int x, int y)
    {
        int cx = x / WorldChunk.Size, cy = y / WorldChunk.Size;
        int lx = x % WorldChunk.Size, ly = y % WorldChunk.Size;
        return ref chunks[cx, cy].Get(lx, ly);
    }
}