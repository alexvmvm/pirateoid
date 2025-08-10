using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour, IGUI
{
    //Const
    private const float CheckInterval = 0.05f; // 20 times per second
    
    //Working vars
    private static Thing thingUnderMouse;
    private List<Region> regionsVisible = new();
    private float lastCheckTime = -1f;
    private List<Interaction> interactions = new();

    //Props
    public Thing ThingUnderMouse => thingUnderMouse;

    public int GUIOrder => UIOrder.Default;

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

    void OnEnable()
    {
        Find.GUIHandler.Register(this);
    }

    void OnDisable()
    {
        Find.GUIHandler.DeRegister(this);
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

        List<Region> regions = RegionsVisible;

        thingUnderMouse = null;
        interactions.Clear();

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

    
        if( thingUnderMouse != null )
        {
            var actor = Find.PlayerController.ControlledThing;
            if( actor != null )
                interactions.AddRange(thingUnderMouse.GetAllInteractions(actor));
        }   
    }

    public void DoGUI()
    {
        if( interactions.Count == 0 )
            return;

        Text.Size = FontSize.Large;
        Text.Anchor = TextAnchor.UpperLeft;
        
        Interaction interaction = interactions[0];
        string text = interaction.label;

        Vector2 mousePosition = RootUI.MousePosition;
        Vector2 size = Text.CalcSize(text);   
        Vector2 offset = new Vector2(10, 0);
        
        Rect rect = new(
            mousePosition.x + offset.x, 
            mousePosition.y + offset.y, 
            size.x, 
            size.y);

        UI.Label(rect, text);
        Text.Size = FontSize.Small;

        if( RootUI.MouseDown )
            interaction.action();
    }

    void Update()
    {
        if( Time.time - lastCheckTime < CheckInterval )
            return;

        lastCheckTime = Time.time;

        CheckUpdateThingUnderMouse();
    }
}
