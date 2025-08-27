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

    //Props
    public Overworld Overworld 
    { 
        get
        {
            if( overworld == null )
                overworld = new Overworld(worldWidth, worldHeight, seed);
            
            return overworld;
        }
    }

    /// <summary>Ensure a world chunk at (cx,cy) has been generated.</summary>
    public void EnsureChunk(int cx, int cy)
    {
        var chunk = Overworld.chunks[cx, cy];
        if (!chunk.dirty)
        {
            OverworldGen.GenerateChunk(Overworld, cx, cy);
            // Note: No rendering/baking here—just pure data gen
        }
    }

    public bool InBoundsWorldTile(int x, int y)
        => x >= 0 && y >= 0 && x < Overworld.width && y < Overworld.height;
}