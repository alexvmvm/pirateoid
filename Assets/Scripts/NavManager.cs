using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class NavManager : MonoBehaviour
{
    private bool[] nodes;
    private GridIndexer indexer;

    public void UpdateRegionNodes(Region region)
    {
        Profiler.BeginSample("UpdateRegionNodes");

        if( nodes == null )
        {
            var width  = Find.Map.width;
            var height = Find.Map.height;

            nodes = new bool[width * height];
            indexer = new GridIndexer(width, height);
        }

        var min = region.rect.min;
        var max = region.rect.max;

        static bool RegionThingContainsPoint(Region region, Vector2 p)
        {
            foreach(var thing in region.thingLister.AllThings)
            {
                if( thing.def.traversability != Traversability.Impassable )
                    continue;
                
                if( thing.Contains(p) )
                    return true;
            }

            return false;
        }

        for(var x = min.x; x < max.x; x++)
        {
            for(var y = min.y; y < max.y; y++)
            {
                var p = new Vector2Int(x, y);

                var index = indexer.ToIndex(p);
                
                nodes[index] = false;

                if( RegionThingContainsPoint(region, p.Center()) )
                    continue;

                nodes[index] = true;
            }
        }

        Profiler.EndSample();
    }

    void OnDrawGizmos()
    {
        if( nodes == null )
            return;

        Gizmos.color = Color.red;

        for(var i = 0; i < nodes.Length; i++)
        {
            if( !nodes[i] )
                continue;
            
            var cell = indexer.ToCell(i);

            Gizmos.DrawWireSphere(new Vector2(cell.x + 0.5f, cell.y + 0.5f), 0.25f);
        }
    }
}
