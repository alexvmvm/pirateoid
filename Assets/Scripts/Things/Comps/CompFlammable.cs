using System;
using UnityEngine;

[Serializable]
public partial class CompProperties_Flammable : CompProperties
{
    public float burnTime = 5f;

    public CompProperties_Flammable()
	{
		compClass = typeof(CompFlammable);
	}
}

public class CompFlammable : ThingComp
{
    public CompFlammable(Thing parent) : base(parent)
    {
    }

    public CompFlammable(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public override void Tick()
    {
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Flammable
{
    public override void DrawEditorFields()
    {
        burnTime    = UnityEditor.EditorGUILayout.FloatField("Burn Time", burnTime);
    }
}
#endif