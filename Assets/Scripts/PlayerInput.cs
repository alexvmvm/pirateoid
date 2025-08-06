using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const float Distance = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if( RaycastUtils.UIToMapPosition(Input.mousePosition, out Vector3 hitPos) )
        {
            foreach(var thing in ThingFinderUtils.GetNearbyThings(hitPos, Distance) )
            {
                //TODO
            }
        }
    }
}

public static class ThingFinderUtils
{
    private static List<Thing> tmpThings = new();

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
                    if( Vector2.Distance(mapPos, things[i].position) < distance )
                    {
                        tmpThings.Add(things[i]);
                    }
                }
            });

        return tmpThings;
    }
}
