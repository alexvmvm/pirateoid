using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class DefOf
{
    private static readonly Dictionary<string, Def> cachedDef = new();

    public static T GetCachedDef<T>(string name) where T : Def
    {
        var type = typeof(T);
        var typeName = type.Name;
        var key = typeName + "_" + name;

        if(!cachedDef.ContainsKey(key))
        {
            cachedDef[key] = Resources.LoadAll<T>("Defs/Things/Buildings").FirstOrDefault(d => (typeName + "_" + d.name) == key);

            if( cachedDef[key] == null )
                UnityEngine.Debug.LogError("Failed to find def " + name);
        }
        
        return (T)cachedDef[key];
    }


    public static void FillReferences<T>(Type type) where T : Def
    {
        foreach (var p in type.GetFields( System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
        {
            p.SetValue(null, GetCachedDef<T>(p.Name));
        }
    }
}

public static class ThingDefOf
{
    public static ThingDef Barricade;

    static ThingDefOf()
    {
        DefOf.FillReferences<ThingDef>(typeof(ThingDefOf));
    }
}