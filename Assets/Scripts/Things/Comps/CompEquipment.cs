using System;
using System.Collections.Generic;

[Serializable]
public partial class CompProperties_Equipment : CompProperties
{
    public CompProperties_Equipment()
	{
		compClass = typeof(CompEquipment);
	}
}

public class CompEquipment : ThingComp
{
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
        //capacity = UnityEditor.EditorGUILayout.IntField("Capacity", capacity);
    }
}
#endif