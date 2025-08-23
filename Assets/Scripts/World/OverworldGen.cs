using UnityEngine;

public static class OverworldGen
{
    // Tunables
    const float CONTINENT_SCALE = 0.0016f;   // lower → larger landmasses
    const float DETAIL_SCALE    = 0.006f;    // coastline detail
    const float WARP_SCALE      = 0.02f;
    const float WARP_STRENGTH   = 0.08f;

    // Generate one chunk’s worth of tiles
    public static void GenerateChunk(Overworld world, int cx, int cy)
    {
        var chunk = world.chunks[cx, cy];
        int ox = cx * WorldChunk.Size;
        int oy = cy * WorldChunk.Size;

        int baseSeed = world.seed;

        for (int y = 0; y < WorldChunk.Size; y++)
        {
            for (int x = 0; x < WorldChunk.Size; x++)
            {
                int wx = ox + x;
                int wy = oy + y;

                // Domain warp
                Vector2 w = Warp(wx, wy, baseSeed);
                float nx = (wx + w.x) * CONTINENT_SCALE;
                float ny = (wy + w.y) * CONTINENT_SCALE;

                // Continents/coastline mask
                float c = FBM(nx, ny, baseSeed + 11, 4, 2.0f, 0.5f);
                float d = FBM((wx + 0.5f) * DETAIL_SCALE, (wy + 0.5f) * DETAIL_SCALE, baseSeed + 23, 3, 2.2f, 0.55f);
                float mask = c * 0.75f + d * 0.25f; // blend

                // Shift threshold to get ocean-dominant world
                bool land = mask > 0.53f;

                // Height (coarse), temperature by latitude, moisture by noise
                short height = (short)Mathf.RoundToInt(Mathf.Lerp(-40f, 120f, mask));
                byte temp    = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * LatitudeTemp(wy, world.height)), 0, 255);
                byte moist   = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * Moisture(wx, wy, baseSeed + 77)), 0, 255);

                int islandId = land ? LabelIsland(wx, wy, baseSeed) : -1;
                WorldBiome biome = ClassifyBiome(land, temp, moist, height);

                ref var t = ref world.Get(wx, wy);
                t.height   = height;
                t.temp     = temp;
                t.moist    = moist;
                t.biome    = biome;
                t.islandId = islandId;
                t.poi      = land && IsPOICandidate(wx, wy, baseSeed);
            }
        }

        chunk.dirty = true;
    }

    // --- Helpers -------------------------------------------------------------

    static float FBM(float x, float y, int seed, int oct = 4, float lac = 2f, float gain = 0.5f)
    {
        float a = 0.5f, f = 1f, sum = 0f, amp = 1f, totalAmp = 0f;
        for (int i = 0; i < oct; i++)
        {
            float n = Mathf.PerlinNoise(x * f + seed * 0.001f, y * f + seed * 0.001f);
            sum += n * amp;
            totalAmp += amp;
            f *= lac; amp *= gain;
        }
        return sum / Mathf.Max(0.0001f, totalAmp);
    }

    static Vector2 Warp(int x, int y, int seed)
    {
        float nx = x * WARP_SCALE, ny = y * WARP_SCALE;
        float wx = Mathf.PerlinNoise(nx + seed, ny) - 0.5f;
        float wy = Mathf.PerlinNoise(nx, ny + seed) - 0.5f;
        return new Vector2(wx, wy) * WARP_STRENGTH * WorldChunk.Size;
    }

    static float LatitudeTemp(int y, int worldH)
    {
        float lat = Mathf.InverseLerp(0, worldH, y);   // 0 south → 1 north
        float equator = 1f - Mathf.Abs(lat - 0.5f) * 2f; // 1 at equator, 0 at poles
        return Mathf.Clamp01(equator * 0.85f + 0.15f);
    }

    static float Moisture(int x, int y, int seed)
    {
        return FBM(x * 0.004f, y * 0.004f, seed, 4, 2.2f, 0.55f);
    }

    static bool IsPOICandidate(int x, int y, int seed)
    {
        // Low-probability deterministic flag
        int h = HashUtils.Combine(seed, x, y);
        return (h & 1023) == 0; // ~1/1024 chance
    }

    // Cheap island label via hashed cell centers (not true Voronoi, but stable)
    static int LabelIsland(int x, int y, int seed)
    {
        int cellSize = 192; // larger → sparser island centers
        int cx = Mathf.FloorToInt((float)x / cellSize);
        int cy = Mathf.FloorToInt((float)y / cellSize);
        return Mathf.Abs(HashUtils.Combine(seed, cx, cy));
    }

    static WorldBiome ClassifyBiome(bool land, byte t, byte m, short h)
    {
        if (!land) return (h > -10 && h < 5) ? WorldBiome.Reef : WorldBiome.Ocean;

        float tf = t / 255f;
        float mf = m / 255f;

        if (h > 80) return WorldBiome.Mountain;
        if (h < 5)  return WorldBiome.Beach;

        if (tf > 0.7f && mf > 0.5f) return WorldBiome.Tropical;
        if (tf > 0.6f && mf < 0.35f) return WorldBiome.Desert;
        if (tf < 0.3f) return WorldBiome.Tundra;
        return WorldBiome.Temperate;
    }
}