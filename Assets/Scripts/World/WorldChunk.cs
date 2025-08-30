using UnityEngine;

public enum WorldBiome : byte { Ocean, Reef, Beach, Tropical, Temperate, Desert, Tundra, Mountain }

public struct WorldTile
{
    public short height;      // -100..+100 (coarse “meters” or arbitrary units)
    public byte  temp;        // 0..255
    public byte  moist;       // 0..255
    public WorldBiome biome;  // coarse biome
    public int   islandId;    // -1 ocean; >=0 = island label
    public bool  poi;         // point-of-interest flag
}

public class WorldChunk
{
    public const int Size = 128; // coarse tiles per chunk
    public bool dirty = true;
    public readonly WorldTile[] tiles = new WorldTile[Size * Size];

    public ref WorldTile Get(int x, int y) => ref tiles[y * Size + x];
}