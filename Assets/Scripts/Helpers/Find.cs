using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class Find
{
    //Props
    public static Map Map                       => GetObjectCached(ref map);
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
    public static Selector Selector                 => GetObjectCached(ref selector);
    public static ReservationManager Reservations   => GetObjectCached(ref reservations);
    public static PathFinder PathFinder             => GetObjectCached(ref pathFinder);
    public static FrameUpdate FrameUpdate           => GetObjectCached(ref frameUpdate);
    public static World World                       => GetObjectCached(ref world);
    public static WorldRenderer WorldRenderer       => GetObjectCached(ref worldRenderer);

    //Props - misc
    public static TileMap TileMap                   => Map.TileMap;
    public static UniqueIdManager UniqueIdManager   => uniqueIdManager ??= new UniqueIdManager();
    public static Camera Camera                     => camera ??= Camera.main;


    //Private cache
    private static Map map;
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
    private static UniqueIdManager uniqueIdManager;
    private static Selector selector;
    private static Camera camera;
    private static ReservationManager reservations;
    private static PathFinder pathFinder;
    private static FrameUpdate frameUpdate;
    private static World world;
    private static WorldRenderer worldRenderer;

    public static void Reset()
    {
        map = null;
        spriteManager = null;
        regionManager = null;
        navManager = null;
        canvasScaler = null;
        guiHandler = null;
        windowManager = null;
        debugTool = null;
        playerController = null;
        ticker = null;
        uniqueIdManager = null;
        selector = null;
        camera = null;
        reservations = null;
        pathFinder = null;
        frameUpdate = null;
        worldRenderer = null;
    }
    
    
    private static T GetObjectCached<T>(ref T obj, bool includeInactive = false) where T : MonoBehaviour
    {
        return obj ??= Object.FindObjectOfType<T>(includeInactive);
    }
}
