using System;
using System.Collections.Generic;

public enum EquipmentSlot
{
    None,
    Weapon
}

[Serializable]
public partial class CompProperties_Equipment : CompProperties
{
    public EquipmentSlot slot = default;

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
        slot = (EquipmentSlot)UnityEditor.EditorGUILayout.EnumPopup("Slot", slot);
    }
}
#endif