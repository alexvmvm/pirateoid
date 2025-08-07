using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private const float Distance = 0.5f;
    private static List<Thing> thingsUnderMouse = new();

    public bool IsSelectable(Thing thing)
    {
        return thing.DrawBounds.Contains(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        thingsUnderMouse.Clear();

        Vector3 mouse = Input.mousePosition;

        if( RaycastUtils.UIToMapPosition(mouse, out Vector3 hitPos) )
        {
            foreach(var thing in ThingFinderUtils.GetNearbyThings(hitPos, Distance) )
            {
                thingsUnderMouse.Add(thing);
            }
        }
    }
}
