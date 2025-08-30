using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("World Data")]
    public int worldWidth  = 8192;
    public int worldHeight = 4096;
    public int seed        = 123456;

    //Working vars 
    private Overworld overworld;
    private WorldRenderer worldRenderer;
    private WorldNoise noise;

    //Props
    public bool Visible => worldRenderer.enabled;
    public Overworld Overworld 
    { 
        get
        {
            if( overworld == null )
                overworld = new Overworld(worldWidth, worldHeight);
            
            return overworld;
        }
    }

    void Awake()
    {
        worldRenderer = GetComponent<WorldRenderer>();
        noise = GetComponent<WorldNoise>();
    }

    public void ToggleVisible()
    {
        worldRenderer.enabled = !worldRenderer.enabled;
    }

    /// <summary>Ensure a world chunk at (cx,cy) has been generated.</summary>
    public void EnsureChunk(int cx, int cy)
    {
        var chunk = Overworld.chunks[cx, cy];
        if (chunk.dirty)
        {
            noise.GenerateChunk(Overworld, cx, cy, seed);
            chunk.dirty = false;
            
            // Note: No rendering/baking here—just pure data gen
        }
    }

    public bool InBoundsWorldTile(int x, int y)
        => x >= 0 && y >= 0 && x < Overworld.width && y < Overworld.height;

    [ContextMenu("Regenerate")]
    public void Regenerate()
    {
        var overWorld = Overworld;

        for(var x = 0; x < overWorld.chunksX; x++)
        {
            for(var y = 0; y < overWorld.chunksY; y++)
            {
                overWorld.chunks[x, y].dirty = true;
            }
        }
        
        worldRenderer.ClearCache();
    }

    [ContextMenu("Regenerate (and seed)")]
    public void RegenerateAndSeed()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        
        Regenerate();
    }
}