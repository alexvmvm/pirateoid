using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public static class UI
{
    //Const 
    private const int ItemCountTextSize = 18;
    public const int DefaultRowHeight = 15;
    public const int ProgressBarHeight = 10;
    public const float DefaultLineThickness = 0.5f;
    public const int BtnHeight = 20;
    public const int HeaderHeight = 16;
    public static readonly Texture2D WhiteTexture = CreateTexture(Color.white);
    public static readonly Texture2D PanelBackgroundTexture = CreateTexture(PanelBackgroundColor);
    public static readonly Texture2D ArrowDown = Resources.Load<Texture2D>("Textures/UI/arrow-down");
    public static readonly Texture2D ArrowUp = Resources.Load<Texture2D>("Textures/UI/arrow-up");
    public static readonly Texture2D ArrowRight = Resources.Load<Texture2D>("Textures/UI/arrow-right");
    public static readonly Texture2D CheckboxCheckedTexture = Resources.Load<Texture2D>("Textures/UI/checkbox-checked");
    public static readonly Texture2D CheckboxTexture = Resources.Load<Texture2D>("Textures/UI/checkbox");
    public static readonly Color32 PanelBackgroundColor = new Color32(20, 23, 28, 255);
    public static readonly Color32 PanelBackgroundLightColor = new Color32(25, 28, 32, 255);
    public static readonly Color32 PanelBorderColor = new Color32(46, 48, 51, 255);
    public static readonly Color32 PanelHighlight = new(40, 40, 40, 255);
    public static readonly Color32 BorderColor = new Color32(15, 17, 22, 255);
    public static readonly Color32 HighlightColor = new Color32(155, 155, 155, 55);
    public static readonly Color32 HeaderBorderColor = new(15, 17, 22, 255);
    public static readonly Color32 HeaderBackgroundColor = new(10, 12, 17, 255);
    public static readonly Color32 InspectPaneTextColor = new(40, 40, 40, 255);
    public static readonly Color32 SubHeaderBackgroundColor = new(140, 140, 140, 255);
    public static readonly Color32 SubHeaderBorderColor = new(110, 110, 110, 255);
    public static readonly Color32 DebugColor = new(255, 154, 45, 255);
    public static readonly Color32 RedReadable = new (230, 41, 41, 255);
    public static readonly Color32 YellowReadable = new (230, 195, 41, 255);
    public static readonly Color32 GreenReadable = new (88, 158, 30, 255);
    public static readonly Color32 GrayReadable = new(82, 81, 81, 255);
    public static readonly Color32 TextColorSubHeading = new(251, 248, 254, 255);
    public static readonly Texture2D PauseTexture = Resources.Load<Texture2D>("Textures/UI/pause");
    public static readonly Texture2D PlayTexture = Resources.Load<Texture2D>("Textures/UI/play");
    public static readonly Texture2D PlayFastTexture = Resources.Load<Texture2D>("Textures/UI/fast");
    public static readonly Texture2D PlayFastestTexture = Resources.Load<Texture2D>("Textures/UI/fastest");
    public static readonly Texture2D PlaceholderTexture = Resources.Load<Texture2D>("Textures/UI/placeholder-ui");
    public const int GapSmall = 1;
    public const int Gap = 2;
    public const int GapLarge = 5;

    //Working vars
    private static Matrix4x4 matrix4X4;
    private static GUIStyle itemCountTextStyle;

    static UI()
    {
        itemCountTextStyle = new(GUI.skin.label)
        {
            alignment = TextAnchor.LowerLeft
        };
    }

    public static void BeginGUI()
    {
        matrix4X4 = GUI.matrix;
        
        GUI.matrix = Matrix4x4.TRS( new Vector3(0, 0, 0), Quaternion.identity, new Vector3(RootUI.Scale, RootUI.Scale, 1));
    }   

    public static void EndGUI()
    {
        GUI.matrix = matrix4X4;
    }

    private static Texture2D CreateTexture(Color color)
    {
        var texture = new Texture2D(1, 1); 	
        texture.SetPixel(0, 0, color);
        texture.Apply();	
        return texture;
    }

    public static void DoItemCount(Rect rect, string text)
    {
        // Get the camera's orthographic size (for orthographic camera) or field of view (for perspective camera)
        Camera camera = Camera.main;
        float cameraZoom = camera.orthographic ? camera.orthographicSize : camera.fieldOfView;

        // Define a base font size
        int baseFontSize = 14;

        // Scale the font size inversely with the camera zoom to keep text size constant
        float scaleFactor = 1f / (cameraZoom / 5f);  // Adjust 5 as necessary to fit your scene's scale
        int scaledFontSize = Mathf.RoundToInt(baseFontSize * scaleFactor);

        var style = itemCountTextStyle;
        style.fontSize = scaledFontSize;
        style.contentOffset = new Vector2(0, 2); // Try tweaking the Y value

        GUI.Label(rect, text, style);
    }

    public static void Label(Rect rect, string text)
    {
        GUI.Label(rect, text, Text.CurFontStyle);
    }

    public static void Header(Rect rect, string text)
    {
        GUI.color = HeaderBackgroundColor;
        Box(rect);
        GUI.color = Color.white;

        Text.Anchor = TextAnchor.MiddleLeft;
        Text.Size = FontSize.Medium;
        Label(rect.ContractBy(GapLarge, 0), text);
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Size = FontSize.Small;

        // GUI.color = HeaderBorderColor;
        // Border(rect);
        // GUI.color = Color.white;
    }

    public static void DrawTexture(Rect rect, Texture texture, ScaleMode scaleMode = ScaleMode.ScaleAndCrop)
    {
        GUI.DrawTexture(rect, texture, scaleMode);
    }

    public static void ProgressBar(Rect rect, float fill, Color? barColor = null, Color? backgroundColor = null)
    {
        GUI.color = backgroundColor ?? Color.gray;
        Box(rect);
        rect.SplitHorizontallyPercent(out Rect left, out Rect _, fill);
        GUI.color = barColor ?? Colors.SoftGreen;
        Box(left.ContractBy(1));
        GUI.color = Color.white;
    }

    public static void Box(Rect rect)
    {        
        DrawTexture(rect, WhiteTexture, ScaleMode.StretchToFill);
    }

    public static bool RadioButton(Rect rect)
    {
        Circle(rect.center, Mathf.Min(rect.width, rect.height));

        return ButtonHidden(rect);
    }

    public static bool RadioButtonWithLabel(Rect rect, string label)
    {
        var labelRect = new Rect(rect.x + rect.height + GapSmall, rect.y, rect.width - rect.height - GapSmall, rect.height);
        Label(labelRect, label);

        return RadioButton(new Rect(rect.x, rect.y, rect.height, rect.height));
    }

    public static void Circle(Vector2 center, float radius, float thickness = 1f, float segments = 10f)
    {
        //TODO: use texture
        // var arc = Mathf.PI * 2 / segments;

        // for(var i = 0f; i <= Mathf.PI * 2; i += arc)
        // {
        //     var p1 = new Vector2(
        //         center.x + radius * Mathf.Sin(i),
        //         center.y + radius * Mathf.Cos(i)
        //     );

        //     var p2 = new Vector2(
        //         center.x + radius * Mathf.Sin(i + arc),
        //         center.y + radius * Mathf.Cos(i + arc)
        //     );

        //     Line(p1, p2, thickness);
        // }

        throw new NotImplementedException();
    }

    public static bool Button(Rect rect, Texture texture, bool background = false, bool active = false)
    {       
        if( background )
        {
            GUI.color = PanelBackgroundColor;
            Box(rect);
            GUI.color = Color.white;
        }
            
        
        DrawTexture(rect.ContractBy(GapSmall), texture, ScaleMode.ScaleToFit);

        if( active )
        {
            GUI.color = Color.white;
            Border(rect);
        }

        return ButtonHidden(rect);
    }

    public static bool Button(Rect rect, string label, bool active = true, Color? backgroundColor = null, Color? labelColor = null)
    {       
        GUI.color = backgroundColor ?? PanelBackgroundColor;
        Box(rect);
        GUI.color = Color.white;

        GUI.color = Color.gray;
        Border(rect);
        GUI.color = Color.white;

        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = labelColor ?? Color.white;
        Label(rect, label);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;

        if( active && RootUI.IsOver(rect) )
            Highlight(rect);

        return active && ButtonHidden(rect);
    }

    public static bool ButtonHidden(Rect rect)
    {
        if(rect.Contains(RootUI.MousePosition))
        {
            if( RootUI.MouseUp )
            {
                Event.current.Use();
                return false;
            }

            if( RootUI.MouseDown )
            {
                //SoundUtility.PlaySound("UI/click1", "UI/click2");
                Event.current.Use();
                return true;
            }
        }
        
        return false;
    }

    public static bool CheckboxWithLabel(Rect rect, string label, bool selected, Color? labelColor = null)
    {
        var labelRect = new Rect(rect.x + rect.height + Gap, rect.y, rect.width - rect.height - Gap, rect.height);
        
        GUI.color = labelColor ?? Color.white;
        Text.Anchor = TextAnchor.MiddleLeft;
        Label(labelRect, label);
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height).ContractBy(Gap), selected ? CheckboxCheckedTexture : CheckboxTexture);

        return ButtonHidden(rect);
    }

    public static bool Checkbox(Rect rect, bool selected)
    {
        DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height).ContractBy(Gap), selected ? CheckboxCheckedTexture : CheckboxTexture);

        return ButtonHidden(rect);
    }

    public static bool ToggleGroup(Rect rect, string label, bool open, Color? labelColor = null)
    {
        var labelRect = new Rect(rect.x + rect.height + Gap, rect.y, rect.width - rect.height - Gap, rect.height);
        
        GUI.color = labelColor ?? Color.white;
        Text.Anchor = TextAnchor.MiddleLeft;
        Label(labelRect, label);
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height).ContractBy(Gap), open ? ArrowDown : ArrowRight);

        if( RootUI.IsOver(rect) )
            Highlight(rect);
    
        return ButtonHidden(rect);
    }

    public static void Border(Rect rect, float thickness = DefaultLineThickness)
    {        
        DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), WhiteTexture);
        DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), WhiteTexture);
        DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), WhiteTexture);
        DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), WhiteTexture);
    }

    public static void LineVertical(Vector2 start, float length, float thickness = DefaultLineThickness)
    {
        DrawTexture(new Rect(start.x, start.y, thickness, length), WhiteTexture);
    }

    public static void LineHorizontal(Vector2 start, float length, float thickness = DefaultLineThickness)
    {
        DrawTexture(new Rect(start.x, start.y, length, thickness), WhiteTexture);
    }

    public static void DrawLine(Vector2 start, Vector2 end, float width = DefaultLineThickness)
	{
        float diffX = end.x - start.x;
        float diffY = end.y - start.y;
        float len = Mathf.Sqrt(diffX * diffX + diffY * diffY);

        if(len < 0.01f) return;
        
        float wDiffX = width * diffY / len;
        float wDiffY = width * diffX / len;

        float angle = -Mathf.Atan2(-diffY, diffX) * Mathf.Rad2Deg;
        
        var pivot = start + new Vector2(0.5f * wDiffX, -0.5f * wDiffY);
        var rotationMatrix = Matrix4x4.TRS((Vector3) pivot, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one) * Matrix4x4.TRS((Vector3) (-pivot), Quaternion.identity, Vector3.one);

        Rect translatedRect = new Rect( start.x, start.y - 0.5f * wDiffY, len, width);
        
        GL.PushMatrix();
        GL.MultMatrix(rotationMatrix);
	    GUI.DrawTexture(translatedRect, WhiteTexture);
        GL.PopMatrix();
    }

    public static void Highlight(Rect rect)
    {
        GUI.color = HighlightColor;
        Box(rect);
        GUI.color = Color.white;
    }

    public static string TextField(Rect rect, string value)
    {
        GUI.color = new Color32(255, 255, 255, 155);
        Border(rect);
        GUI.color = Color.white;

        return GUI.TextField(rect.ContractBy(GapSmall, 0), value, Text.CurFontStyle);
    }

    public static void TextRow(float x, ref float y, float width, string text)
    {
        Text.WordWrap = true;

        var height = Text.CalcHeight(text, width);
        var rect = new Rect(x, y, width, height);

        UI.Label(rect, text);   

        Text.WordWrap = false;
        y += height;
    }

    public static void TextRowHighlighted(float x, ref float y, float width, string text, Color color)
    {
        var pad = UI.Gap;
        Text.WordWrap = true;
        var height = Text.CalcHeight(text, width - (2 * pad));

        var rect = new Rect(x, y, width, height + (2 * pad));
        var colorBefore = GUI.color;
        GUI.color = color;
        Border(rect);
        GUI.color = colorBefore;

        GUI.color = new Color(color.r, color.g, color.b, 0.1f);
        Box(rect);
        GUI.color = Color.white;

        y += pad;
        TextRow(x + pad, ref y, width - (2 * pad), text);
        y += pad;
        
        Text.WordWrap = false;
    }

    public static int DrawTabs<T>(Rect rect, int selectedTabIndex, List<T> tabs, Func<T, string> labelGetter, Func<T, Texture> iconGetter = null)
    {
        float tabWidth = rect.width / tabs.Count;

        for (int i = 0; i < tabs.Count; i++)
        {
            Rect tab = new Rect(rect.x + i * tabWidth, rect.y, tabWidth, rect.height);

            bool isSelected = i == selectedTabIndex;
            Color bgColor = isSelected ? UI.PanelHighlight : UI.PanelBackgroundColor;

            var label = labelGetter(tabs[i]);
            var texture = iconGetter != null ? iconGetter(tabs[i]) : null;

            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Weight = FontWeight.Bold;

            var labelWidth = Text.CalcSize(label).x;
            var totalWidth = labelWidth + (texture != null ? tab.height : 0f);
            var x = tab.x + (tab.width - totalWidth) / 2f;

            if (texture != null)
            {
                UI.DrawTexture(new Rect(x, tab.yMin, tab.height, tab.height).ContractBy(4), texture);
                x += tab.height;
            }

            UI.Label(new Rect(x, tab.yMin, labelWidth, tab.height), labelGetter(tabs[i]));
            Text.Weight = FontWeight.Normal;
            Text.Anchor = TextAnchor.UpperLeft;
            
            DrawTexture(new Rect(tab.xMin, tab.yMin, tab.width, DefaultLineThickness), WhiteTexture);
            if( !isSelected )
                DrawTexture(new Rect(tab.xMin, tab.yMax - DefaultLineThickness, tab.width, DefaultLineThickness), WhiteTexture);
            DrawTexture(new Rect(tab.xMin, tab.yMin, DefaultLineThickness, tab.height), WhiteTexture);
            DrawTexture(new Rect(tab.xMax - DefaultLineThickness, tab.yMin, DefaultLineThickness, tab.height), WhiteTexture);

            if (ButtonHidden(tab))
            {
                selectedTabIndex = i;
            }
        }

        return selectedTabIndex;
    }
}
