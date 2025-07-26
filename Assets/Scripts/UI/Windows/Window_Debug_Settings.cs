using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;

public static class DebugSettings
{
    [DebugHeading("Camera")]
    [DebugGroupToggle("Camera")]
    public static bool cameraOrthographic;

    [DebugGroupToggle("Camera")]
    public static bool cameraPerspective;
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class DebugHeadingAttribute : PropertyAttribute
{
    public string Label;

    public DebugHeadingAttribute(string label)
    {
        Label = label;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class DebugGroupToggleAttribute : Attribute
{
    public string GroupLabel;

    public DebugGroupToggleAttribute(string groupLabel)
    {
        GroupLabel = groupLabel;
    }
}

public class Window_Debug_Settings : Window
{
    public override float Width  => UIScreen.Width;
    public override float Height => UIScreen.Height;

    private List<List<DebugAction>> active = new();
    private List<FieldInfo> fieldInfo;
    private Dictionary<string, List<FieldInfo>> fieldInfoByGroup;

    //Props
    private List<FieldInfo> Fields
    {
        get
        {
            if( fieldInfo.NullOrEmpty() )
            {
                fieldInfo ??= new();
                fieldInfo.AddRange(typeof(DebugSettings).GetFields(BindingFlags.Static | BindingFlags.Public));
            }
            
            return fieldInfo;
        }
    }

    private Dictionary<string, List<FieldInfo>> FieldsByGroup
    {
        get
        {
            if( fieldInfoByGroup.NullOrEmpty() )
            {
                fieldInfoByGroup ??= new();
                
                foreach (var field in Fields)
                {
                    var attr = field.GetCustomAttribute<DebugGroupToggleAttribute>();
                    if (attr != null)
                    {
                        if (!fieldInfoByGroup.ContainsKey(attr.GroupLabel))
                            fieldInfoByGroup[attr.GroupLabel] = new List<FieldInfo>();
                        
                        fieldInfoByGroup[attr.GroupLabel].Add(field);
                    }
                }

            }
            
            return fieldInfoByGroup;
        }
    }

    public Window_Debug_Settings()
    {
        this.pausesGame = false;
        this.closeOnEsc  = false;

        this.backgroundColor = new Color32(32, 32, 32, 155);
    }

    public override void DoWindowContents(Rect rect)
    {
        var x = UI.Gap;
        var y = UI.Gap;

        //Const
        const int Width = 80;
        const int Height = 12;

        // Groups
        var fields = Fields;

        // Toggles
        for(var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];

            var headingAttr = field.GetCustomAttribute<DebugHeadingAttribute>();
            if( headingAttr != null)
            {
                Text.Weight = FontWeight.Bold;
                Text.Anchor = TextAnchor.MiddleLeft;
                UI.Label(new Rect(x, y, Width, Height), headingAttr.Label); // or a custom style
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Weight = FontWeight.Normal;

                y += Height + UI.Gap;
            }

            if (field.FieldType == typeof(bool))
            {
                bool current = (bool)field.GetValue(null);

                Rect toggleRect = new Rect(x, y, Width, Height);
                if( UI.CheckboxWithLabel(toggleRect, field.Name.CamelCaseToLabel(), current) )
                {
                    FieldToggled(field, current);
                }

                y += Height + UI.Gap;
            }
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

    private void FieldToggled(FieldInfo field, bool currentValue)
    {
        var groups = FieldsByGroup;

        var group = field.GetCustomAttribute<DebugGroupToggleAttribute>();
        if( group != null )
        {
            var label = group.GroupLabel;
            if( label != null && groups.ContainsKey(label) )
            {
                foreach(var fi in groups[label])
                {
                    fi.SetValue(null, false);
                }

                field.SetValue(null, true);
                return;
            }
        }

        field.SetValue(null, !currentValue);

    }
}
