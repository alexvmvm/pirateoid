using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtons : MonoBehaviour, IGUI
{
    public int GUIOrder => UIOrder.Default;

    void OnEnable()
    {
        Find.GUIHandler.Register(this);
    }

    void OnDisable()
    {
        Find.GUIHandler.DeRegister(this);
    }

    public void DoGUI()
    {
        var rect = new Rect(UIScreen.Width - 50 - UI.Gap, UIScreen.Height - 20 - UI.Gap, 50, 20);
        if( UI.Button(rect, "World") )
        {
            Find.World.ToggleVisible();
        }
    }
}
