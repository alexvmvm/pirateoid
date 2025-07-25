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
    public static CanvasScaler CanvasScaler     => GetObjectCached(ref canvasScaler);
    public static GUIHandler GUIHandler         => GetObjectCached(ref guiHandler);
    public static WindowManager WindowManager   => GetObjectCached(ref windowManager);
    public static DebugTool DebugTool           => GetObjectCached(ref debugTool);
    public static PlayerController PlayerController => GetObjectCached(ref playerController);
    public static CameraController CameraController => GetObjectCached(ref cameraController);
    public static Ticker Ticker                     => GetObjectCached(ref ticker);

    //Private cache
    private static NavMeshManager navMeshManager;
    private static SpriteManager spriteManager;
    private static RegionManager regionManager;
    private static NavManager navManager;
    private static CanvasScaler canvasScaler;
    private static GUIHandler guiHandler;
    private static WindowManager windowManager;
    private static DebugTool debugTool;
    private static PlayerController playerController;
    private static CameraController cameraController;
    private static Ticker ticker;

    public static void Reset()
    {
        navMeshManager = null;
        spriteManager = null;
        regionManager = null;
        navManager = null;
        canvasScaler = null;
        guiHandler = null;
        windowManager = null;
        debugTool = null;
        playerController = null;
        ticker = null;
    }
    
    
    private static T GetObjectCached<T>(ref T obj, bool includeInactive = false) where T : MonoBehaviour
    {
        return obj ??= Object.FindObjectOfType<T>(includeInactive);
    }
}
