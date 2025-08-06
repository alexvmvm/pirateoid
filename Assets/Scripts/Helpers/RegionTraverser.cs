
using System;
using UnityEngine;

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