using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionNodes
{
    public List<Vector2> points = new();
}

public class NavManager : MonoBehaviour
{
    //Const
    private const float HighNodeDensity = 10;

    //Working vars
    public Dictionary<Region, RegionNodes> nodesByRegions = new();

    public void UpdateRegionNodes(Region region)
    {
        if( !nodesByRegions.ContainsKey(region) )
            nodesByRegions[region] = new();
        
        var nodes = nodesByRegions[region];

        nodes.points.Clear();
        
        var min = region.rect.min;
        var max = region.rect.max;

        static bool RegionThingContainsPoint(Region region, Vector2 p)
        {
            foreach(var thing in region.thingLister.AllThings)
            {
                if( thing.Contains(p) )
                    return true;
            }

            return false;
        }

        for(var x = min.x; x < max.x; x++)
        {
            for(var y = min.y; y < max.y; y++)
            {
                var p = new Vector2(x + 0.5f, y + 0.5f);

                if( RegionThingContainsPoint(region, p) )
                    continue;

                nodes.points.Add(p);
            }
        }

        for(var i = nodes.points.Count - 1; i >= 0; i--)
        {
            var nearThing = false;
            foreach(var thing in region.thingLister.AllThings)
            {
                if( Vector2.Distance(GeomUtils.ClosestPointOnPolygon(nodes.points[i], thing.Corners), nodes.points[i]) < 2f )
                {
                    nearThing = true;
                    break;
                }
            }

            if( !nearThing )
                nodes.points.RemoveAt(i);
        }
        
    }

    void OnDrawGizmos()
    {
        foreach((Region region, RegionNodes nodes) in nodesByRegions)
        {
            foreach(var p in nodes.points)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(p, 0.2f);
            }
        }
    }
}
