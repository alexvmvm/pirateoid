using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour, IGUI
{
    //Working vars
    private readonly List<Window> activeWindows = new();

    //Props
    public int GUIOrder => global::UIOrder.Window;
    public bool PauseGame
    {
        get
        {
            for(var i = 0; i < activeWindows.Count; i++)
            {
                if( activeWindows[i].pausesGame )
                    return true;
            }
            
            return false;
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

    public void Add<T>(T window) where T : Window
    {
        if( GetActiveWindow<T>() != null )
        {
            Debug.LogError("Window already active.");
            return;
        }

        activeWindows.Add(window);
    }

    public void Remove<T>(T window) where T : Window
    {
        activeWindows.Remove(window);
    }

    public void RemoveIfActive<T>() where T : Window
    {
        var window = GetActiveWindow<T>();
        if( window != null )
            Remove(window);
    }

    public bool IsWindowActive<T>() where T : Window
    {
        return GetActiveWindow<T>() != null;
    }

    public T GetActiveWindow<T>() where T : Window
    {
        for(var i = 0; i < activeWindows.Count; i++)
        {
            if( activeWindows[i] is T dialog )
                return dialog;
        }

        return null;
    }

    public bool AnyActiveWindowContains(Vector2 position)
    {
        for(var i = 0; i < activeWindows.Count; i++)
        {
            if( activeWindows[i].GetDrawRect().Contains(position))
                return true;
        }

        return false;
    }

    public void DoGUI()
    {
        if( RootUI.Esc )
        {
            if( activeWindows.Count == 0 )
                Add(new Window_MainMenu());
            else
            {
                var lastWindow = activeWindows[^1];
                if( lastWindow.closeOnEsc )
                    lastWindow.Close();
            }
        }

        for(var i = 0; i < activeWindows.Count; i++)
        {
            activeWindows[i].DoWindow(activeWindows[i].GetDrawRect());
        }
    }
}