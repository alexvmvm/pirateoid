using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : MonoBehaviour, IGUI
{
    //Props
    public int GUIOrder => default;

    //Working vars
    private DebugAction action;

    void OnEnable()
    {
        Find.GUIHandler.Register(this);
    }

    void OnDisable()
    {
        Find.GUIHandler.DeRegister(this);
    }

    public void SetTool(DebugAction action)
    {
        this.action = action;
    }

    public void DoGUI()
    {
        if( action == null )
            return;
        
        UI.Label(new Rect(RootUI.MousePosition + Vector2.right * 10, new(200, 20)), action.label);

        if( RootUI.MouseDown )
            action.action();
        
        if( RootUI.MouseContext )
            action = null;
    }
}
