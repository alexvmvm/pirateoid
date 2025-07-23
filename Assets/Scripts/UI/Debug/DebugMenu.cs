using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour, IGUI
{
    //Const
    private const int BtnWidth = 60;
    private const int BtnHeight = 12;

    //Props
    public int GUIOrder => GUIDepth.Window;

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
        var x = UIScreen.Width/2f - BtnWidth/2f;
        var y = 0;

        if( UI.Button(new Rect(x, y, BtnWidth, BtnHeight), "Debug actions") )
            ToggleDebugWindow();
        x += BtnWidth + UI.Gap;
        
        // if( UI.CheckboxWithLabel(new Rect(x, y, 45, BtnHeight), "God mode", Find.Debug.Enabled) )
        //     Find.Debug.ToggleEnabled();
        // x += 45 + UI.Gap;

        // var algoActive = Find.WindowManager.GetActiveWindow<Window_Debug_PassengerAlgorithm>() is not null;
        // if( UI.CheckboxWithLabel(new Rect(x, y, BtnWidth, BtnHeight), "Passenger algo", algoActive) )
        //     ToggleAlgo();
    }

    private void ToggleDebugWindow()
    {
        var window = Find.WindowManager.GetActiveWindow<Window_Debug_Options>() ;
        if( Find.WindowManager.GetActiveWindow<Window_Debug_Options>() != null )
            Find.WindowManager.Remove(window);
        else
            Find.WindowManager.Add(new Window_Debug_Options());
    }
}
