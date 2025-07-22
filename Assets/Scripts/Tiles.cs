using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Tile
{
    public int typeId;
    public byte elevation;
    public bool walkable;
}

public static class TileDatabase
{
    public static TileDef[] All;

    public static void Initialize()
    {
        All = Resources.LoadAll<TileDef>("Defs/Tiles")
            .OrderBy(def => def.name) // or any order you want
            .ToArray();

        for (int i = 0; i < All.Length; i++)
        {
            All[i].id = i;
        }
    }

    public static TileDef Get(int id) 
    {
        if( All == null )
            Initialize();

        return All[id];
    }
}

public enum TileType
{
    None,
    Floor,
    Wall
}

public enum TileLayer : byte
{
    Floor = 0,
    Wall = 1,
    Roof = 2,
    // Add more as needed
    Count
}

public class TileChunk
{
    public const int ChunkSize = 16;
    public const int LayerCount = (int)TileLayer.Count;

    public bool dirty = false;
    private readonly Tile[] tiles = new Tile[ChunkSize * ChunkSize * LayerCount];

    public ref Tile Get(TileLayer layer, int x, int y)
    {
        int index = ((int)layer * ChunkSize * ChunkSize) + y * ChunkSize + x;
        return ref tiles[index];
    }
}

public class TileMap
{
    public int mapWidth, mapHeight;
    public int chunksX, chunksY;
    public TileChunk[,] chunks;

    public TileMap(int mapWidth, int mapHeight)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        chunksX = Mathf.CeilToInt((float)mapWidth / TileChunk.ChunkSize);
        chunksY = Mathf.CeilToInt((float)mapHeight / TileChunk.ChunkSize);

        chunks = new TileChunk[chunksX, chunksY];
        for (int y = 0; y < chunksY; y++)
            for (int x = 0; x < chunksX; x++)
                chunks[x, y] = new TileChunk();
    }

    public ref Tile GetTile(TileLayer layer, int x, int y)
    {
        int cx = x / TileChunk.ChunkSize;
        int cy = y / TileChunk.ChunkSize;
        int lx = x % TileChunk.ChunkSize;
        int ly = y % TileChunk.ChunkSize;
        return ref chunks[cx, cy].Get(layer, lx, ly);
    }

    public void SetTile(TileLayer layer, int x, int y, TileDef def)
    {
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
        {
            Debug.LogWarning($"SetTile out of bounds: ({x},{y})");
            return;
        }

        int chunkX = x / TileChunk.ChunkSize;
        int chunkY = y / TileChunk.ChunkSize;
        int localX = x % TileChunk.ChunkSize;
        int localY = y % TileChunk.ChunkSize;

        var chunk = chunks[chunkX, chunkY];

        Tile tile = new Tile
        {
            typeId = def.id,
            elevation = 0,
            walkable = def.walkable
        };

        chunk.Get(layer, localX, localY) = tile;
        chunk.dirty = true;

        if( !def.walkable )
            Find.NavMeshManager.RegisterBlocker(new Rect(x, y, 1, 1));
    }
}