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

    public static bool VisibleToCamera(this Region region)
    {
        var cam = Find.Camera;
        
        Vector2 min = region.rect.min;
        Vector2 max = region.rect.max;

        Vector3 bl = cam.WorldToScreenPoint(new Vector3(min.x, min.y, 0f));
        Vector3 tr = cam.WorldToScreenPoint(new Vector3(max.x, max.y, 0f));

        Rect screenRect = cam.CalculateCameraScreenRect();

        // Return true if the region's screen rect intersects the camera's screen rect
        bool xOverlap = bl.x < screenRect.xMax && tr.x > screenRect.xMin;
        bool yOverlap = bl.y < screenRect.yMax && tr.y > screenRect.yMin;

        return xOverlap && yOverlap;
    }
}