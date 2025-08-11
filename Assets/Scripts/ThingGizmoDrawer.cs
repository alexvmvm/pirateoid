using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingGizmoDrawer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        var regions = Find.RegionManager.All;
        
        for(int i = 0; i < regions.Count; i++)
        {
            var things = regions[i].thingLister.AllThings;

            for(int j = 0; j < things.Count; j++)
            {
                things[j].DrawGizmos();
            }
        }
    }
}
