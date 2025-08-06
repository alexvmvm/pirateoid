using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const float Distance = 0.5f;
    private static List<Thing> thingsUnderMouse = new();

    public bool IsUnderMouse(Thing thing)
    {
        return thingsUnderMouse.Contains(thing);
    }

    // Update is called once per frame
    void Update()
    {
        thingsUnderMouse.Clear();

        if( RaycastUtils.UIToMapPosition(Input.mousePosition, out Vector3 hitPos) )
        {
            foreach(var thing in ThingFinderUtils.GetNearbyThings(hitPos, Distance) )
            {
                thingsUnderMouse.Add(thing);
            }
        }
    }
}
