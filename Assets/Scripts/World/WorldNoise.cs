using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNoise : MonoBehaviour
{
    public float continentScale = 0.01f;
    public float continentThreshold = 0.5f;

    public void GenerateChunk(Overworld world, int cx, int cy, int seed)
    {
        var chunk = world.chunks[cx, cy];
        int ox = cx * WorldChunk.Size;
        int oy = cy * WorldChunk.Size;

        var rand = new System.Random(seed);

        // Generate random offsets from the seed
        float offsetX = (float)rand.NextDouble() * 10000f;
        float offsetY = (float)rand.NextDouble() * 10000f;

        float offsetX2 = (float)rand.NextDouble() * 10000f;
        float offsetY2 = (float)rand.NextDouble() * 10000f;

        for (int y = 0; y < WorldChunk.Size; y++)
        {
            for (int x = 0; x < WorldChunk.Size; x++)
            {
                int wx = ox + x;
                int wy = oy + y;

                float continentNoise = Mathf.PerlinNoise(
                    wx * continentScale + offsetX, 
                    wy * continentScale + offsetY);
                
                //float coastlineNoise = Mathf.PerlinNoise(x * 0.03f + offsetX2, y * 0.03f + offsetY2);

                float finalValue = continentNoise; // + coastlineNoise * 0.25f; // small impact on landmass interiors

                bool isLand = finalValue > continentThreshold;
                
                ref var t = ref world.Get(wx, wy);
                t.biome = isLand ? WorldBiome.Beach : WorldBiome.Ocean;
            }
        }
    }
}
