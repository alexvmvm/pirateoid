using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public interface IGUI
{
    int GUIOrder { get; }
    void DoGUI();
}

public class GUIHandler : MonoBehaviour
{
    private readonly List<IGUI> gui = new();

    public void Register(IGUI gui)
    {
        if( !this.gui.Contains(gui) )
        {
            this.gui.Add(gui);
            this.gui.SortByDescending(u => u.GUIOrder);
        }
    }

    public void DeRegister(IGUI gui)
    {
        if( this.gui.Contains(gui) )
            this.gui.Remove(gui);
    }

    private static List<IGUI> tmpGui = new();

    void OnGUI()
    {
        UI.BeginGUI();

        // Make sure we copy as some things might be removed from 
        // gui when we perform a UI action
        tmpGui.Clear();
        tmpGui.AddRange(gui);

        for(var i = 0; i < tmpGui.Count; i++)
        {
            tmpGui[i].DoGUI();
        }

        tmpGui.Clear();

        UI.EndGUI();
    }
}