using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    //Const
    private const float Distance = 0.5f;
    
    //Working vars
    private static Thing thingUnderMouse;
    private List<Region> regionsVisible = new();

    //Props
    public Thing ThingUnderMouse => thingUnderMouse;
    private List<Region> RegionsVisible
    {
        get
        {
            regionsVisible.Clear();

            var camera = Find.Camera;
            var bounds = camera.CalculateCameraScreenRect();

            var regions = Find.RegionManager.All;
            for(var i = 0; i < regions.Count; i++)
            {
                if( bounds.Overlaps(regions[i].rect.ToRect().ToScreenRect(camera)) )
                    regionsVisible.Add(regions[i]);
            }

            return regionsVisible;
        }
    }

    public bool IsSelectable(Thing thing)
    {
        return thingUnderMouse == thing;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        var regions = RegionsVisible;

        thingUnderMouse = null;

        for(var i = 0; i < regions.Count; i++)
        {
            var things = regions[i].thingLister.AllThings;

            for(var j = 0; j < things.Count; j++)
            {
                if( things[j].DrawBounds.Contains(mouse) )
                {
                    if( thingUnderMouse == null || thingUnderMouse.position.y > things[j].position.y )
                        thingUnderMouse = things[j];
                }
            }
        }
    }
}
