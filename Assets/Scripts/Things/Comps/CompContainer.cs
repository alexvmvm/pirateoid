using System;
using System.Collections.Generic;

public interface IThingOwner
{
    Thing OwnerThing { get; }                 // The Thing that owns/holds items (e.g., the pawn/building)
    CompContainer OwningContainer { get; }    // The specific container comp on that Thing
}

[Serializable]
public partial class CompProperties_Container : CompProperties
{
    public int capacity = 10;

    public CompProperties_Container()
	{
		compClass = typeof(CompContainer);
	}
}

public class CompContainer : ThingComp, IThingOwner
{
    private List<Thing> contents = new();

    //Props
    public IReadOnlyList<Thing> Contents => contents;
    public Thing OwnerThing => parent;
    public CompContainer OwningContainer => this;

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

            thing.HeldContainer?.Remove(thing);
            contents.Add(thing);
            thing.SetOwner(this);
        }
    }

    public bool CanAdd(Thing thing)
    {
        // prevent cycles: parent cannot indirectly contain itself
        for(Thing t = parent; t != null; t = t.ThingOwner)
        {
            if (t == thing) 
                return false;
        }
        
        return true;
    }

    public bool Remove(Thing thing)
    {
        if( !contents.Remove(thing) )
            return false;
        
        thing.ClearOwner();
        return true;
    }

    public bool Contains(Thing thing)
    {
        return contents.Contains(thing);
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