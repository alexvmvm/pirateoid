using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static readonly Dictionary<Vector2, Mesh> quadCache = new();
    private Material spriteMaterial;
    private readonly List<Thing> things = new();
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        spriteMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void Register(Thing thing)
    {
        things.Add(thing);
    }

    public void DeRegister(Thing thing)
    {
        things.Remove(thing);
    }

    private static Mesh GetQuad(Vector2 size)
    {
        if (quadCache.TryGetValue(size, out var mesh)) 
            return mesh;

        var w = size.x * 0.5f;
        var h = size.y * 0.5f;

        mesh = new Mesh
        {
            vertices = new[]
            {
                new Vector3(-w, -h),
                new Vector3( w, -h),
                new Vector3( w,  h),
                new Vector3(-w,  h)
            },
            uv = new[]
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1)
            },
            triangles = new[] { 0, 1, 2, 2, 3, 0 }
        };
        mesh.RecalculateNormals();
        quadCache[size] = mesh;
        return mesh;
    }

    private void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Bounds camBounds = CalculateCameraBounds(cam);

        foreach (var thing in things)
        {
            var def = thing.def;
            if (def?.sprite == null) continue;

            Vector3 thingPos = new Vector3(thing.position.x, thing.position.y, 0f);
            Vector2 halfSize = def.size * 0.5f;
            Bounds thingBounds = new Bounds(thingPos, new Vector3(def.size.x, def.size.y, 0.1f));

            if (!camBounds.Intersects(thingBounds))
                continue; // cull invisible sprite

            var mesh  = GetQuad(def.size);

            mpb ??= new();
            mpb.Clear();
            mpb.SetTexture("_MainTex", def.sprite.texture);

            var r = def.sprite.rect;
            var tex = def.sprite.texture;
            mpb.SetVector("_MainTex_ST",
                new Vector4(r.width / tex.width, r.height / tex.height,
                            r.x     / tex.width,  r.y      / tex.height));

            var rotation = Find.CameraController.Mode == CameraMode.Perspective ? 
                Quaternion.LookRotation(cam.transform.forward, Vector3.up) :
                Quaternion.Euler(0, 0, thing.rotation);

            var offset = Find.CameraController.Mode == CameraMode.Perspective ? 
                cam.transform.up * (def.size.y / 2f)  :
                new Vector3();

            var matrix = Matrix4x4.TRS(thingPos + offset, rotation, Vector3.one * def.scale);
            Graphics.DrawMesh(mesh, matrix, spriteMaterial, 0, null, 0, mpb);
        }
    }

    private static Bounds CalculateCameraBounds(Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        cam.CalculateFrustumCorners(
            new Rect(0, 0, 1, 1),
            Mathf.Abs(cam.transform.position.z),
            Camera.MonoOrStereoscopicEye.Mono,
            corners);

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, 0f);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, 0f);

        for (int i = 0; i < 4; i++)
        {
            Vector3 world = cam.transform.TransformPoint(corners[i]);
            min = Vector3.Min(min, world);
            max = Vector3.Max(max, world);
        }

        // Pad slightly to avoid edge popping
        const float padding = 1f;
        min -= Vector3.one * padding;
        max += Vector3.one * padding;

        return new Bounds((min + max) * 0.5f, max - min);
    }
}