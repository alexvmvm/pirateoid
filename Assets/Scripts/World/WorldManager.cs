using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("World")]
    public int worldWidth  = 8192;
    public int worldHeight = 4096;
    public int seed = 123456;

    [Header("Rendering")]
    public float tileWorldSize = 1.0f;       // how big a world tile is in world units
    public Gradient biomeGradient;           // assign in inspector for pretty colors

    public Overworld World { get; private set; }
    public bool Active => orthoCam != null && orthoCam.gameObject.activeInHierarchy;

    // Cache one texture + mesh per chunk
    private readonly Dictionary<(int cx,int cy), Texture2D> chunkTex = new();
    private Mesh chunkQuad;
    private Material chunkMat;
    private MaterialPropertyBlock mpb;
    private Camera orthoCam;
    private int overworldLayer;

    void Awake()
    {
        World = new Overworld(worldWidth, worldHeight, seed);

        // one fullscreen-UV quad (size = chunk tiles * tileWorldSize)
        chunkQuad = BuildQuad(WorldChunk.Size * tileWorldSize, WorldChunk.Size * tileWorldSize);
        chunkMat  = new Material(Shader.Find("Unlit/Texture"));
        mpb       = new MaterialPropertyBlock();

        // If you render with a dedicated camera, consider:
        // Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        // Camera.main.transparencySortAxis = new Vector3(0, 0, -1);

        overworldLayer = LayerMask.NameToLayer("Overworld"); // create in Project Settings → Tags & Layers

        // Create orthographic camera
        var go = new GameObject("OverworldCam");
        orthoCam = go.AddComponent<Camera>();
        orthoCam.orthographic = true;
        orthoCam.orthographicSize = 50f;           // tune to taste / zoom
        orthoCam.cullingMask = 1 << overworldLayer;
        orthoCam.clearFlags = CameraClearFlags.Depth; // keep the scene, draw map over it
        orthoCam.nearClipPlane = -100f;
        orthoCam.farClipPlane  =  100f;

        // Position it to look straight down the Z axis (2D convention)
        var main = Camera.main;
        go.transform.position = new Vector3(main.transform.position.x, main.transform.position.y, -10f);
        go.transform.rotation = Quaternion.identity;

        // BUILT-IN RP: ensure this draws after the main camera
        orthoCam.depth = (main ? main.depth : 0) + 1;
        orthoCam.gameObject.SetActive(false);

    #if UNITY_RENDER_PIPELINE_UNIVERSAL
        // URP camera stacking: make ortho an Overlay, stack on main
        var mainData  = main.GetUniversalAdditionalCameraData();
        var orthoData = orthoCam.GetUniversalAdditionalCameraData();
        orthoData.renderType = UnityEngine.Rendering.Universal.CameraRenderType.Overlay;
        mainData.cameraStack.Add(orthoCam);
    #endif
    }

    public void ToggleVisible()
    {
        orthoCam.gameObject.SetActive(!orthoCam.gameObject.activeSelf);
    }

    void Update()
    {
        // Render visible chunks (very simple view culling)
        if( orthoCam == null || !orthoCam.gameObject.activeInHierarchy ) 
            return;

        Rect view = CalculateViewRect(orthoCam); // in world units

        int minWX = Mathf.Max(0, Mathf.FloorToInt(view.xMin / tileWorldSize));
        int maxWX = Mathf.Min(World.width - 1,  Mathf.CeilToInt(view.xMax / tileWorldSize));
        int minWY = Mathf.Max(0, Mathf.FloorToInt(view.yMin / tileWorldSize));
        int maxWY = Mathf.Min(World.height - 1, Mathf.CeilToInt(view.yMax / tileWorldSize));

        int minCX = minWX / WorldChunk.Size;
        int maxCX = maxWX / WorldChunk.Size;
        int minCY = minWY / WorldChunk.Size;
        int maxCY = maxWY / WorldChunk.Size;

        for (int cy = minCY; cy <= maxCY; cy++)
        for (int cx = minCX; cx <= maxCX; cx++)
        {
            EnsureChunk(cx, cy);
            DrawChunk(cx, cy);
        }
    }

    // --- Public API ----------------------------------------------------------

    public void EnsureChunk(int cx, int cy)
    {
        var chunk = World.chunks[cx, cy];
        if (!chunk.dirty)
        {
            OverworldGen.GenerateChunk(World, cx, cy);
            BakeChunkTexture(cx, cy);
        }
    }

    // public TileMap GenerateLocalMapForIsland(int islandId, int size = 256)
    // {
    //     // Derive island seed → produce local map. Replace with your own IslandGenerator.
    //     int islandSeed = HashUtils.Combine(World.seed, islandId);
    //     var local = new TileMap(size, size);

    //     for (int y = 0; y < size; y++)
    //     for (int x = 0; x < size; x++)
    //     {
    //         float h = Mathf.PerlinNoise((x + islandSeed) * 0.03f, (y - islandSeed) * 0.03f);
    //         var def = h < 0.25f ? TileDefOf.Sand : h < 0.6f ? TileDefOf.Grass : TileDefOf.Rock;
    //         local.SetTile(TileLayer.Floor, x, y, def);
    //     }

    //     return local;
    // }

    // --- Rendering helpers ---------------------------------------------------

    private void BakeChunkTexture(int cx, int cy)
    {
        var key = (cx, cy);
        if (!chunkTex.TryGetValue(key, out var tex))
        {
            tex = new Texture2D(WorldChunk.Size, WorldChunk.Size, TextureFormat.RGBA32, false, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            chunkTex[key] = tex;
        }

        var chunk = World.chunks[cx, cy];
        var pixels = new Color32[WorldChunk.Size * WorldChunk.Size];

        for (int y = 0; y < WorldChunk.Size; y++)
        for (int x = 0; x < WorldChunk.Size; x++)
        {
            ref var t = ref chunk.Get(x, y);
            Color col = ColorForTile(t);
            pixels[y * WorldChunk.Size + x] = col;
        }

        tex.SetPixels32(pixels);
        tex.Apply(false, false);
    }

    private void DrawChunk(int cx, int cy)
    {
        var key = (cx, cy);
        if (!chunkTex.TryGetValue(key, out var tex)) 
            return;

        Vector3 pos = new Vector3(
            (cx * WorldChunk.Size + WorldChunk.Size * 0.5f) * tileWorldSize,
            (cy * WorldChunk.Size + WorldChunk.Size * 0.5f) * tileWorldSize,
            0f);

        mpb.Clear();
        mpb.SetTexture("_MainTex", tex);

        var matrix = Matrix4x4.TRS(pos, Quaternion.identity,
            new Vector3(1f, 1f, 1f)); // mesh already sized in BuildQuad

        Graphics.DrawMesh(chunkQuad, matrix, chunkMat, overworldLayer, orthoCam, 0, mpb);
    }

    private static Mesh BuildQuad(float w, float h)
    {
        var mesh = new Mesh
        {
            vertices = new[]
            {
                new Vector3(-w/2, -h/2, 0),
                new Vector3( w/2, -h/2, 0),
                new Vector3( w/2,  h/2, 0),
                new Vector3(-w/2,  h/2, 0),
            },
            uv = new[]
            {
                new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)
            },
            triangles = new[] { 0, 2, 1, 0, 3, 2 } // flipped winding
        };
        return mesh;
    }

    private Rect CalculateViewRect(Camera cam)
    {
        // Simple orthographic assumption; for perspective, compute frustum corners
        if (cam.orthographic)
        {
            float h = cam.orthographicSize * 2f;
            float w = h * cam.aspect;
            Vector3 c = cam.transform.position;
            return new Rect(c.x - w/2, c.y - h/2, w, h);
        }
        // Fallback: large rect
        return new Rect(-9999, -9999, 19998, 19998);
    }

    private Color ColorForTile(in WorldTile t)
    {
        // If you provide a Gradient in inspector, map by biome. Otherwise, reasonable defaults.
        switch (t.biome)
        {
            case WorldBiome.Ocean:     return new Color(0.08f, 0.25f, 0.45f);
            case WorldBiome.Reef:      return new Color(0.15f, 0.5f, 0.55f);
            case WorldBiome.Beach:     return new Color(0.86f, 0.77f, 0.52f);
            case WorldBiome.Tropical:  return new Color(0.14f, 0.55f, 0.22f);
            case WorldBiome.Temperate: return new Color(0.22f, 0.6f, 0.26f);
            case WorldBiome.Desert:    return new Color(0.82f, 0.72f, 0.40f);
            case WorldBiome.Tundra:    return new Color(0.75f, 0.82f, 0.86f);
            case WorldBiome.Mountain:  return new Color(0.5f, 0.5f, 0.5f);
            default:                   return Color.magenta;
        }
    }

    void LateUpdate()
    {
        if (!orthoCam || !orthoCam.gameObject.activeInHierarchy) 
            return;
        
        var main = Camera.main;
        orthoCam.transform.position = new Vector3(main.transform.position.x, main.transform.position.y, -10f);
        // Optionally tie zoom: orthoCam.orthographicSize = some function of main zoom
    }
}