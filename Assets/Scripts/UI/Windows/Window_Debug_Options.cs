using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public enum DebugActionType
{
    Immediate,
    MapClick
}

public class DebugAction
{    
    //Config
    public string label;
    public Action action;
    public List<DebugAction> children = new();
    public DebugActionType type = DebugActionType.Immediate;

    public DebugAction(string label, Action action)
    {
        this.label = label;
        this.action = action;
    }

    public DebugAction(string label, params DebugAction[] actions)
    {
        this.label = label;
        this.children = new();
        this.children.AddRange(actions);
    }

    public DebugAction(string label, IEnumerable<DebugAction> actions)
    {
        this.label = label;
        this.children = new();
        this.children.AddRange(actions);
    }

    public void DoGUI(float x, float y)
    {

    }
}


public class Window_Debug_Options : Window
{
    public override float Width  => UIScreen.Width;
    public override float Height => UIScreen.Height;

    private List<List<DebugAction>> active = new();

    public Window_Debug_Options()
    {
        this.pausesGame = false;
        this.closeOnEsc  = false;

        this.active.Clear();
        this.active.Add(GetActions().ToList());

        this.backgroundColor = new Color32(32, 32, 32, 155);
    }

    IEnumerable<DebugAction> GetActions()
    {
        //Things
        {
            var things = new DebugAction("Spawn things");

            foreach(var def in Resources.LoadAll<ThingDef>("Defs") )
            {
                var localDef = def;
                var spawn = new DebugAction($"Spawn {localDef.label}");
                spawn.type = DebugActionType.MapClick;
                spawn.action = () =>
                {                    
                    if( RaycastUtils.UIToMapPosition(Input.mousePosition, out Vector3 pos) )
                        ThingSpawner.SpawnThing(localDef, pos);                    
                };

                things.children.Add(spawn);
            }

            yield return things;
        }
    }

    public override void DoWindowContents(Rect rect)
    {
        var x = UI.Gap;
        var y = UI.Gap;

        //Const
        const int Width = 60;
        const int Height = 12;

        var actions = active[^1];

        foreach(var action in actions )
        {
            var r = new Rect(x, y, Width, Height);

            if( UI.Button(r, action.label) )
            {
                if( !action.children.NullOrEmpty() )
                    active.Add(action.children);
                else
                {
                    switch(action.type)
                    {
                        case DebugActionType.Immediate:
                        {
                            if( action.action != null )
                                action.action();       
                        }
                        break;
                        case DebugActionType.MapClick:
                        {
                            Find.WindowManager.RemoveIfActive<Window_Debug_Options>();
                            Find.DebugTool.SetTool(action);
                        }
                        break;
                    }
                }
            }

            y += Height + UI.Gap;
        }

        if( RootUI.Esc )
        {
            if( active.Count > 1 )
            {
                active.RemoveAt(active.Count - 1);
                Event.current.Use();
            }
            else
                Close();
        }
    }
}
