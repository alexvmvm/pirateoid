using System;
using System.Collections.Generic;
using UnityEngine;

public static class RegionUtils
{
    public static IEnumerable<Region> GetNeighbours(this Region region)
    {
        var regions = Find.RegionManager;
        
        for(var i = 0; i < 4; i++)
        {
            var r = regions.GetRegion(region.rect.min + Cells.Cardinal[i] * RegionManager.RegionSize);
            if( r != null )
                yield return r;
        }
    }
}

public static class RegionTraverser
{
    public static void TraverseRegions(Region start, Func<Region, bool> pass, Action<Region> action)
    {
        FloodFillerUtils.FloodFill(start, pass, action, r => r.GetNeighbours());
    }

    public static void TraverseRegions(Vector2Int start, Func<Region, bool> pass, Action<Region> action)
    {
        var region = Find.RegionManager.GetRegion(start);
        if( region != null )
            FloodFillerUtils.FloodFill(region, pass, action, r => r.GetNeighbours());
    }
}