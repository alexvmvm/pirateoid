using System;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    None,
    Weapon
}

[Serializable]
public partial class CompProperties_Equipment : CompProperties
{
    public EquipmentSlot slot = default;
    public GraphicData equippedGraphicSide;
    public GraphicData equippedGraphic;

    public CompProperties_Equipment()
	{
		compClass = typeof(CompEquipment);
	}
}

public class CompEquipment : ThingComp
{
    public CompProperties_Equipment Props => (CompProperties_Equipment)props;

    public CompEquipment(Thing parent) : base(parent)
    {
    }

    public CompEquipment(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public override void Tick()
    {
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Equipment
{
    public override void DrawEditorFields()
    {
        DoGraphicDataFields("Equipped graphic", equippedGraphic);
        DoGraphicDataFields("Equipped side graphic", equippedGraphicSide);
    }

    private static void DoGraphicDataFields(string label, GraphicData graphicData)
    {
        UnityEditor.EditorGUILayout.LabelField(label, UnityEditor.EditorStyles.boldLabel);
        UnityEditor.EditorGUILayout.BeginVertical("box");
        graphicData.sprite = (Sprite)UnityEditor.EditorGUILayout.ObjectField("Sprite", graphicData.sprite, typeof(Sprite), false);
        graphicData.scale = UnityEditor.EditorGUILayout.FloatField("Scale", graphicData.scale);
        graphicData.brightness = UnityEditor.EditorGUILayout.FloatField("Brightness", graphicData.brightness);
        UnityEditor.EditorGUILayout.EndVertical();
    }
}
#endif