using System;
using UnityEngine;

public static class UIButtons
{
    public const int LeftMouse = 0;
    public const int RightMouse = 1;
    public const int MiddleMouse = 2;
}

public static class UIScreen
{
    public static float Width => Screen.width / RootUI.Scale;
    public static float Height => Screen.height / RootUI.Scale;
}

public static class RootUI
{
    public static bool MouseDown => IsEvent(EventType.MouseDown, UIButtons.LeftMouse);
    public static bool MouseDrag => IsEvent(EventType.MouseDrag, UIButtons.LeftMouse);
    public static bool MouseUp => IsEvent(EventType.MouseUp, UIButtons.LeftMouse);
    public static bool MouseContext => IsEvent(EventType.MouseDown, UIButtons.RightMouse);
    public static bool Delete => IsKeyDown(KeyCode.Delete);
    public static bool Esc => IsKeyDown(KeyCode.Escape);
    public static Vector2 MousePosition => Event.current.mousePosition;
    
    //Props
    public static float Scale 
    {
        get
        {
            // Reference resolution from the CanvasScaler
            Vector2 referenceResolution = Find.CanvasScaler.referenceResolution;

            // Match setting from CanvasScaler (0 = width, 1 = height)
            var match = Find.CanvasScaler.matchWidthOrHeight;

            // Calculate scaling factors for width and height
            var widthScale = Screen.width / referenceResolution.x;
            var heightScale = Screen.height / referenceResolution.y;

            // Interpolate between the two scales based on the match setting
            return Mathf.Lerp(widthScale, heightScale, match) * ScaleFactor;
        }
    }
    private const float ScaleFactor = 0.85f ;

    public static bool IsKeyDown(KeyCode code)
    {
        return  
            Event.current != null &&
            Event.current.type == EventType.KeyDown &&
            Event.current.keyCode == code; 
    }
    
    public static bool IsOver(Rect rect)
    {
        return rect.Contains(Event.current.mousePosition);
    }

    public static bool IsMouseDown(Rect rect)
    {
        return MouseDown && IsOver(rect);
    }

    public static bool IsMouseContextDown(Rect rect)
    {
        return MouseContext && IsOver(rect);
    }

    public static bool IsEvent(EventType type, int button = -1) // 0 = left, 1 = right, 2 = middle, > 2 = other buttons
    {
        return  
            Event.current != null &&
            Event.current.type == type &&
            Event.current.rawType == type &&
            (button < 0 || Event.current.button == button); 
    }
    
    
    public static KeyCode NumberToKeyCode(int number)
    {
        switch(number)
        {
            case 1: return KeyCode.Alpha1;
            case 2: return KeyCode.Alpha2;
            case 3: return KeyCode.Alpha3;
            case 4: return KeyCode.Alpha4;
            case 5: return KeyCode.Alpha5;
            case 6: return KeyCode.Alpha6;
            case 7: return KeyCode.Alpha7;
            case 8: return KeyCode.Alpha8;
            case 9: return KeyCode.Alpha9;
        }

        throw new Exception($"Cannot map number {number} to key ");
    }
}  
