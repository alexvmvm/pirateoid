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