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
        spriteMaterial = new Material(Shader.Find("Custom/UnlitSpriteTint"));
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
            colors = new[]
            {
                Color.white,
                Color.white,
                Color.white,
                Color.white
            },
            triangles = new[] { 0, 1, 2, 2, 3, 0 }
        };
        mesh.RecalculateNormals();
        quadCache[size] = mesh;
        return mesh;
    }

    private static (Sprite sprite, bool flipped, float scale) GetSprite(Thing thing)
    {
        if( thing.def.thingType == ThingType.Pawn )
        {
            return thing.FacingDirection switch
            {
                FacingDirection.North   => (thing.def.graphicBack != null ? thing.def.graphicBack.sprite : thing.def.graphicData.sprite, false, thing.def.graphicBack.scale),
                FacingDirection.East    => (thing.def.graphicSide != null ? thing.def.graphicSide.sprite : thing.def.graphicData.sprite, false, thing.def.graphicSide.scale),
                FacingDirection.West    => (thing.def.graphicSide != null ? thing.def.graphicSide.sprite : thing.def.graphicData.sprite, true, thing.def.graphicSide.scale),
                _                       => (thing.def.graphicData.sprite, false, thing.def.graphicData.scale)
            };
        }

        return (thing.def.graphicData.sprite, false, thing.def.graphicData.scale);
    }

    private void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Bounds camBounds = CalculateCameraBounds(cam);

        foreach (var thing in things)
        {
            var def = thing.def;
            
            (Sprite sprite, bool flipped, float scale) = GetSprite(thing);
            
            if( sprite == null )
                continue;
            
            Vector3 thingPos    = new Vector3(thing.position.x, thing.position.y, 0f);
            Bounds thingBounds  = new Bounds(thingPos, new Vector3(def.size.x, def.size.y, 0.1f));

            if (!camBounds.Intersects(thingBounds))
                continue; // cull invisible sprite

            var mesh  = GetQuad(def.size);

            mpb ??= new();
            mpb.Clear();
            mpb.SetTexture("_MainTex", sprite.texture);

            var r = sprite.rect;
            var tex = sprite.texture;
            mpb.SetVector("_MainTex_ST",
                new Vector4(r.width / tex.width, r.height / tex.height,
                            r.x     / tex.width,  r.y      / tex.height));
            
            mpb.SetColor("_Color", Find.PlayerInput.IsUnderMouse(thing) ? Color.red : Color.white);

            var rotation = Find.CameraController.Mode == CameraMode.Perspective ? 
                Quaternion.LookRotation(cam.transform.forward, Vector3.up) :
                Quaternion.Euler(0, 0, thing.rotation);

            var offset = Find.CameraController.Mode == CameraMode.Perspective ? 
                cam.transform.up * (def.size.y / 2f)  :
                new Vector3();

            var flipScale  = flipped ? new Vector3(-1f, 1f, 1f) : Vector3.one;
            var finalScale = Vector3.Scale(flipScale, Vector3.one * scale);
            
            var matrix = Matrix4x4.TRS(thingPos + offset, rotation, finalScale);
            
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