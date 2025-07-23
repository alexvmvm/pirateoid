using System;
using UnityEngine;

public enum FontSize
{
    Small,
    Medium,
    Large
}

public enum FontWeight
{
    Normal,
    Bold,
    Italic,
    BoldItalic
}

public static class Text
{
    public static FontSize Size { get; set; }
    public static FontWeight Weight { get; set; } = FontWeight.Normal;
    public static TextAnchor Anchor { get; set; }
    public static bool WordWrap { get; set; }

    private static GUIStyle[] fontStyles = new GUIStyle[3];
    private static GUIStyle[] textFieldStyles = new GUIStyle[3];
    private static GUIStyle btnStyle;
    private static GUIContent tmpGUIContent;
    
    private const int SmallFontSize = 6;
    private const int MediumFontSize = 8;
    private const int LargeFontSize = 11;

    public static GUIStyle CurFontStyle
	{
		get
		{
            GUIStyle style;
            switch (Size)
			{
				case FontSize.Small:	style = fontStyles[0]; break;
				case FontSize.Medium:	style = fontStyles[1]; break;
				case FontSize.Large:	style = fontStyles[2]; break;
				default: throw new NotImplementedException();
			}

			style.alignment = Anchor;
			style.wordWrap = WordWrap;
            style.fontStyle = ConvertToUnityFontStyle(Weight);

			return style;
		}
	}
    public static GUIStyle CurTextFieldStyle
    {
        get
        {
            switch(Size)
            {
                case FontSize.Small:	return textFieldStyles[0];
                case FontSize.Medium:	return textFieldStyles[1];
                case FontSize.Large:	return textFieldStyles[2];
            }
            throw new NotImplementedException();
        }
    }

    public static GUIStyle CurButtonStyle => btnStyle;

    static Text()
    {
        //GUI.skin = Assets.GetSkin("GUISkin");

        fontStyles[0] = new GUIStyle(GUI.skin.label);
        fontStyles[0].fontSize = SmallFontSize;

        fontStyles[1] = new GUIStyle(GUI.skin.label);
        fontStyles[1].fontSize = MediumFontSize;

        fontStyles[2] = new GUIStyle(GUI.skin.label);
        fontStyles[2].fontSize = LargeFontSize;

        textFieldStyles[0] = new GUIStyle(GUI.skin.textField);
        textFieldStyles[0].fontSize = SmallFontSize;

        textFieldStyles[1] = new GUIStyle(GUI.skin.textField);
        textFieldStyles[1].fontSize = MediumFontSize;

        textFieldStyles[2] = new GUIStyle(GUI.skin.textField);
        textFieldStyles[2].fontSize = LargeFontSize;

        btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = LargeFontSize;

        tmpGUIContent = new();
    }

    public static float CalcHeight(string text, float width)
    {
        tmpGUIContent.text = text;
        return CurFontStyle.CalcHeight(tmpGUIContent, width);
    }

    public static Vector2 CalcSize(string text)
    {
        tmpGUIContent.text = text;
        return CurFontStyle.CalcSize(tmpGUIContent);
    }

    private static FontStyle ConvertToUnityFontStyle(FontWeight weight)
    {
        return weight switch
        {
            FontWeight.Normal       => FontStyle.Normal,
            FontWeight.Bold         => FontStyle.Bold,
            FontWeight.Italic       => FontStyle.Italic,
            FontWeight.BoldItalic   => FontStyle.BoldAndItalic,
            _                       => FontStyle.Normal
        };
    }
}