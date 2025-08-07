using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static readonly Dictionary<Vector2, Mesh> quadCache = new();
    private Material spriteMaterial;
    private readonly List<Thing> things = new();
    private MaterialPropertyBlock mpb;

    private static Material outlineMaterial;
    private static MaterialPropertyBlock outlineMpb;

    void Awake()
    {
        spriteMaterial = new Material(Shader.Find("Custom/UnlitSpriteTint"));
        outlineMaterial = new Material(Shader.Find("Custom/UnlitSpriteOutline"));
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

    private static (Sprite sprite, bool flipped, float scale, float brightness) GetSprite(Thing thing)
    {
        if( thing.def.thingType == ThingType.Pawn )
        {
            return thing.FacingDirection switch
            {
                FacingDirection.North   => (thing.def.graphicBack != null ? thing.def.graphicBack.sprite : thing.def.graphicData.sprite, false, thing.def.graphicBack.scale, thing.def.graphicBack.brightness),
                FacingDirection.East    => (thing.def.graphicSide != null ? thing.def.graphicSide.sprite : thing.def.graphicData.sprite, false, thing.def.graphicSide.scale, thing.def.graphicSide.brightness),
                FacingDirection.West    => (thing.def.graphicSide != null ? thing.def.graphicSide.sprite : thing.def.graphicData.sprite, true, thing.def.graphicSide.scale, thing.def.graphicSide.brightness),
                _                       => (thing.def.graphicData.sprite, false, thing.def.graphicData.scale, thing.def.graphicData.brightness)
            };
        }

        return (thing.def.graphicData.sprite, false, thing.def.graphicData.scale, thing.def.graphicData.brightness);
    }

    private void Update()
    {
        Rect camBounds = Find.Camera.CalculateCameraScreenRect();

        foreach (var thing in things)
        {
            var def = thing.def;
            
            (Sprite sprite, bool flipped, float scale, float brightness) = GetSprite(thing);
            
            if( sprite == null )
                continue;
            
            Rect thingBounds  = thing.DrawBounds;

            if (!camBounds.Overlaps(thingBounds))
                continue; // cull invisible sprite

            var mesh = GetQuad(def.size);

            mpb ??= new();
            mpb.Clear();
            mpb.SetTexture("_MainTex", sprite.texture);

            var r = sprite.rect;
            var tex = sprite.texture;
            mpb.SetVector("_MainTex_ST",
                new Vector4(r.width / tex.width, r.height / tex.height,
                            r.x     / tex.width,  r.y      / tex.height));

            var rotation = Find.CameraController.Mode == CameraMode.Perspective ? 
                Quaternion.LookRotation(Find.Camera.transform.forward, Vector3.up) :
                Quaternion.Euler(0, 0, thing.rotation);

            var flipScale  = flipped ? new Vector3(-1f, 1f, 1f) : Vector3.one;
            var finalScale = Vector3.Scale(flipScale, Vector3.one * scale);

            Color color = Color.white * brightness;

            if( Find.Selector.IsSelectable(thing) )
            {
                float pulse = Mathf.Sin(Time.time * 6f) * 0.25f + 1.25f;
                color *= pulse; // e.g. (1.25, 1.25, 1.25)

                outlineMpb ??= new();
                outlineMpb.SetTexture("_MainTex", sprite.texture);
                outlineMpb.SetColor("_Color", new Color(1f, 1f, 1f, 0.6f));

                var outlineScale = finalScale * 1.1f; // slightly larger
                var outlineMatrix = Matrix4x4.TRS(thing.DrawPos, rotation, outlineScale);
                Graphics.DrawMesh(mesh, outlineMatrix, outlineMaterial, 0, null, 0, outlineMpb);
            }
            
            mpb.SetColor("_Color", color);

            var matrix = Matrix4x4.TRS(thing.DrawPos, rotation, finalScale);
            
            Graphics.DrawMesh(mesh, matrix, spriteMaterial, 0, null, 0, mpb);
        }
    }
}