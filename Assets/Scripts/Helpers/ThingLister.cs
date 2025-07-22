using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ThingGroup
{
    Impassable
}

public class ThingLister 
{
    //Working vars
    private readonly Dictionary<ThingGroup, List<Thing>> things = new();
    private readonly List<Thing> allThings = new();
    private readonly List<ThingGroup> groups = new();

    //Props
    public List<Thing> AllThings => allThings;

    public ThingLister()
    {
        groups.AddRange(Enum.GetValues(typeof(ThingGroup)).Cast<ThingGroup>());

        for(var i = 0; i < groups.Count; i++)
        {
            things[groups[i]] = new();
        }
    }

    public void Add(Thing go)
    {
        foreach(var g in GetGroupsFor(go))
        {
            things[g].Add(go);
        }

        allThings.Add(go);
    }

    public void Remove(Thing go)
    {
        foreach(var g in GetGroupsFor(go))
        {
            things[g].Remove(go);
        }
        
        allThings.Remove(go);
    }

    public bool Contains(Thing gameObject) => AllThings.Contains(gameObject);

    public List<Thing> GetThingsInGroup(ThingGroup group)
    {
        return things[group];
    }

    public int GetThingCountInGroup(ThingGroup group)
    {
        return things[group].Count;
    }
    
    private IEnumerable<ThingGroup> GetGroupsFor(Thing go)
    {
        for(var i = 0; i < groups.Count; i++)
        {
            if( go.IsInGroup(groups[i]) )
                yield return groups[i];
        }
    }
}

public static class ThingListerUtils
{
    public static bool IsInGroup(this Thing t, ThingGroup group)
    {
        return group switch
        {
            ThingGroup.Impassable       => t.def.traversability == Traversability.Impassable,
            _                           => false,
        };
    }
}