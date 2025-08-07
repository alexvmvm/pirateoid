using System.Collections.Generic;
using UnityEngine;

public static class ThingFinderUtils
{
    private static readonly List<Thing> tmpThings = new();

    public static List<Thing> GetNearbyThings(Vector2 mapPos, float distance)
    {
        tmpThings.Clear();

        var cell = mapPos.ToVector2IntFloor();
        RegionTraverser.TraverseRegions(cell, 
            r => Vector2Int.Distance(r.rect.ClosestPointInRect(cell), cell) < distance,
            r => 
            {
                var things = r.thingLister.AllThings;

                for(var i = 0; i < things.Count; i++)
                {
                    if( Vector2.Distance(mapPos, things[i].DrawPos) < distance )
                    {
                        tmpThings.Add(things[i]);
                    }
                }
            });

        return tmpThings;
    }
}