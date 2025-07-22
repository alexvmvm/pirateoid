using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class Find
{
    //Props
    public static NavMeshManager NavMeshManager => GetObjectCached(ref navMeshManager);
    public static SpriteManager SpriteManager   => GetObjectCached(ref spriteManager);
    public static RegionManager RegionManager   => GetObjectCached(ref regionManager);
    public static NavManager NavManager         => GetObjectCached(ref navManager);

    //Private cache
    private static NavMeshManager navMeshManager;
    private static SpriteManager spriteManager;
    private static RegionManager regionManager;
    private static NavManager navManager;

    public static void Reset()
    {
        navMeshManager = null;
        spriteManager = null;
        regionManager = null;
        navManager = null;
    }
    
    
    private static T GetObjectCached<T>(ref T obj, bool includeInactive = false) where T : MonoBehaviour
    {
        return obj ??= Object.FindObjectOfType<T>(includeInactive);
    }
}
