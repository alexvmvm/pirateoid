using System;
using System.Collections.Generic;

[Serializable]
public partial class CompProperties_Container : CompProperties
{
    public int capacity = 10;

    public CompProperties_Container()
	{
		compClass = typeof(CompContainer);
	}
}

public class CompContainer : ThingComp
{
    private List<Thing> contents = new();

    public CompContainer(Thing parent) : base(parent)
    {
    }

    public CompContainer(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public void Add(Thing thing)
    {
        if( !contents.Contains(thing) )
        {            
            if( thing.Spawned )
                thing.DeSpawn();
            
            contents.Add(thing);
        }
    }

    public bool CanAdd(Thing thing)
    {
        return true;
    }

    public bool Remove(Thing thing)
    {
        return contents.Remove(thing);
    }

    public override void Tick()
    {
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Container
{
    public override void DrawEditorFields()
    {
        capacity = UnityEditor.EditorGUILayout.IntField("Capacity", capacity);
    }
}
#endif