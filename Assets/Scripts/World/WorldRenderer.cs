using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(World))]
public class WorldRenderer : MonoBehaviour
{
    [Header("Rendering")]
    [Tooltip("How big a world tile is in world units")]
    public float tileWorldSize = 1.0f;

    [Tooltip("Optional color ramp per biome (leave null to use defaults)")]
    public Gradient biomeGradient; // not required; kept for your future styling

    [Header("Camera & Layer")]
    [Tooltip("Layer used for the overworld draw calls")]
    public string overworldLayerName = "Overworld";

    [Tooltip("Orthographic size of the overworld camera")]
    public float orthoSize = 150f;

    [Tooltip("Camera clear flags for the overworld camera")]
    public CameraClearFlags clearFlags = CameraClearFlags.Depth;

    // refs
    private World world;

    // caches for rendering
    private readonly Dictionary<(int cx, int cy), Texture2D> chunkTex = new();
    private Mesh chunkQuad;
    private Material chunkMat;
    private MaterialPropertyBlock mpb;
    private Camera orthoCam;
    private int overworldLayer;

    void Awake()
    {
        world = GetComponent<World>();

        // one fullscreen-UV quad (size = chunk tiles * tileWorldSize)
        chunkQuad = BuildQuad(WorldChunk.Size * tileWorldSize, WorldChunk.Size * tileWorldSize);

        // Pick a shader that exists in your RP (URP first, fallback to Built-in)
        var sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Unlit/Texture");
        if (sh == null) { Debug.LogError("Overworld: no unlit shader found"); return; }

        chunkMat = new Material(sh);
        mpb      = new MaterialPropertyBlock();

        // Make sure backfaces aren’t culled if winding ends up flipped
        if (chunkMat.HasProperty("_Cull")) chunkMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        // Layer
        overworldLayer = LayerMask.NameToLayer(overworldLayerName);
        if (overworldLayer < 0)
        {
            Debug.LogWarning($"Layer '{overworldLayerName}' not found. Using Default layer 0.");
            overworldLayer = 0;
        }

        // Create orthographic camera (childed to this)
        var go = new GameObject("OverworldCam");
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(world.Overworld.width / 2f, world.Overworld.height / 2f, -10f);
        go.transform.rotation = Quaternion.identity;

        orthoCam = go.AddComponent<Camera>();
        orthoCam.orthographic = true;
        orthoCam.orthographicSize = orthoSize;
        orthoCam.cullingMask = 1 << overworldLayer;
        orthoCam.clearFlags = clearFlags;
        orthoCam.nearClipPlane = -100f;
        orthoCam.farClipPlane  =  100f;

        var controller = go.AddComponent<CameraController>();
        controller.maxZoom = 350;
        controller.moveSpeed = 250;
        controller.zoomSpeed = 15;

        orthoCam.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        orthoCam.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        orthoCam.gameObject.SetActive(false);
    }

    void Update()
    {
        // Simple ortho view rect
        Rect view = CalculateViewRect(orthoCam); // world units

        int minWX = Mathf.Max(0, Mathf.FloorToInt(view.xMin / tileWorldSize));
        int maxWX = Mathf.Min(world.Overworld.width  - 1, Mathf.CeilToInt(view.xMax / tileWorldSize));
        int minWY = Mathf.Max(0, Mathf.FloorToInt(view.yMin / tileWorldSize));
        int maxWY = Mathf.Min(world.Overworld.height - 1, Mathf.CeilToInt(view.yMax / tileWorldSize));

        int minCX = minWX / WorldChunk.Size;
        int maxCX = maxWX / WorldChunk.Size;
        int minCY = minWY / WorldChunk.Size;
        int maxCY = maxWY / WorldChunk.Size;

        for (int cy = minCY; cy <= maxCY; cy++)
        for (int cx = minCX; cx <= maxCX; cx++)
        {
            // Ask the data component to ensure chunk exists
            world.EnsureChunk(cx, cy);

            // Ensure baked texture exists/updated
            BakeChunkTexture(cx, cy);

            // Draw it
            DrawChunk(cx, cy);
        }
    }

    // ------------------------------------------------------------------------
    // Rendering helpers
    // ------------------------------------------------------------------------

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

            var chunk = world.Overworld.chunks[cx, cy];
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
    }

    private void DrawChunk(int cx, int cy)
    {
        if (!chunkTex.TryGetValue((cx, cy), out var tex)) 
            return;

        Vector3 pos = new Vector3(
            (cx * WorldChunk.Size + WorldChunk.Size * 0.5f) * tileWorldSize,
            (cy * WorldChunk.Size + WorldChunk.Size * 0.5f) * tileWorldSize,
            0f);

        mpb.Clear();

        // Bind texture property depending on RP
        if (chunkMat.HasProperty("_BaseMap"))
        {
            mpb.SetTexture("_BaseMap", tex);       // URP
            if (chunkMat.HasProperty("_BaseColor")) mpb.SetColor("_BaseColor", Color.white);
        }
        else
        {
            mpb.SetTexture("_MainTex", tex);       // Built-in
            if (chunkMat.HasProperty("_Color")) mpb.SetColor("_Color", Color.white);
        }

        var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
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
            // Winding so the quad faces +Z; with culling off it’s robust either way
            triangles = new[] { 0, 2, 1, 0, 3, 2 }
        };
        return mesh;
    }

    private Rect CalculateViewRect(Camera cam)
    {
        if (cam.orthographic)
        {
            float h = cam.orthographicSize * 2f;
            float w = h * cam.aspect;
            Vector3 c = cam.transform.position;
            return new Rect(c.x - w / 2f, c.y - h / 2f, w, h);
        }
        return new Rect(-9999, -9999, 19998, 19998);
    }

    private Color ColorForTile(in WorldTile t)
    {
        // Your biomeGradient could be applied here based on t.biome/height if you like.
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

    public void ClearCache()
    {
        chunkTex.Clear();
    }
}