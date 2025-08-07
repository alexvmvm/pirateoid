using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    //Const
    private const float CheckInterval = 0.05f; // 20 times per second
    
    //Working vars
    private static Thing thingUnderMouse;
    private List<Region> regionsVisible = new();
    private float lastCheckTime = -1f;

    //Props
    public Thing ThingUnderMouse => thingUnderMouse;
    private List<Region> RegionsVisible
    {
        get
        {
            regionsVisible.Clear();


            var regions = Find.RegionManager.All;
            for(var i = 0; i < regions.Count; i++)
            {
                if( regions[i].VisibleToCamera() )
                    regionsVisible.Add(regions[i]);
            }

            return regionsVisible;
        }
    }

    public bool IsSelectable(Thing thing)
    {
        return thingUnderMouse == thing;
    }

    private bool CanEverSelect(Thing thing)
    {
        if( Find.PlayerController.IsControlled(thing) )
            return false;
        
        return true;
    }

    private void CheckUpdateThingUnderMouse()
    {
        Vector3 mouse = Input.mousePosition;

        var regions = RegionsVisible;

        thingUnderMouse = null;

        for(var i = 0; i < regions.Count; i++)
        {
            var things = regions[i].thingLister.AllThings;

            for(var j = 0; j < things.Count; j++)
            {
                if( !CanEverSelect(things[j]) )
                    continue;
                
                if( things[j].DrawBounds.Contains(mouse) )
                {
                    if( thingUnderMouse == null || thingUnderMouse.position.y > things[j].position.y )
                        thingUnderMouse = things[j];
                }
            }
        }
    }

    void Update()
    {
        if( Time.time - lastCheckTime < CheckInterval )
            return;

        lastCheckTime = Time.time;

        CheckUpdateThingUnderMouse();
    }
}
