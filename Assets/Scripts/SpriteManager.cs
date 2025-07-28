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

        // Build a new quad centred at (0,0) spanning size.x × size.y.
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
        foreach (var thing in things)
        {
            var def = thing.def;
            if (def?.sprite == null) continue;

            // 1. pick (or build) the quad for this size
            var mesh  = GetQuad(def.size);

            // 2. Set sprite texture & sub‑rect
            mpb ??= new();
            mpb.Clear();
            mpb.SetTexture("_MainTex", def.sprite.texture);

            var r = def.sprite.rect;
            var tex = def.sprite.texture;
            mpb.SetVector("_MainTex_ST",
                new Vector4(r.width / tex.width, r.height / tex.height,
                            r.x     / tex.width,  r.y      / tex.height));

            var rotation = Find.CameraController.Mode == CameraMode.Perspective ? 
                Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up) :
                Quaternion.Euler(0, 0, thing.rotation);

            var offset = Find.CameraController.Mode == CameraMode.Perspective ? 
                Camera.main.transform.up * (def.size.y / 2f)  :
                new Vector3();

            // 3. TRS: position & rotation from Thing; scale from Def.scale (size baked into mesh)
            var matrix = Matrix4x4.TRS(
                new Vector3(thing.position.x, thing.position.y, 0f) + offset,
                rotation,
                Vector3.one * def.scale);

            Graphics.DrawMesh(mesh, matrix, spriteMaterial, 0, null, 0, mpb);
        }
    }
}
