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
    public Vector3 eastOffset;

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

    public Vector3 GetEquipmentOffset(FacingDirection facingDirection)
    {
        // direction
        var offset = facingDirection switch
        {
            FacingDirection.East => Props.eastOffset,
            FacingDirection.West => new Vector3(-Props.eastOffset.x, Props.eastOffset.y, Props.eastOffset.z),
            _                    => Vector3.zero
        };

        Vector3 eqOffset = Find.Camera.transform.forward * 0.0002f;

        // z offset
        offset += facingDirection == FacingDirection.North ? eqOffset : -eqOffset;  

        return offset;
    }

    public override void Tick()
    {
    }

    public override void DrawGizmos()
	{
        // if( parent.IsHeld )
        // {
        //     Gizmos.DrawWireSphere(parent.DrawPos, 0.1f);
        // }
	}
}

#if UNITY_EDITOR
public partial class CompProperties_Equipment
{
    public override void DrawEditorFields()
    {
        DoGraphicDataFields("Equipped graphic", equippedGraphic);
        DoGraphicDataFields("Equipped side graphic", equippedGraphicSide);

        eastOffset = UnityEditor.EditorGUILayout.Vector3Field("East offset", eastOffset);
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