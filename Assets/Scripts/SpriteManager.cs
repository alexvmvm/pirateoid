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

    // Picks the correct GraphicData for this thing/facing and whether it should be flipped.
    private static (GraphicData data, bool flipped) GetGraphicDataFor(Thing thing)
    {
        var def = thing.def;

        if (def.thingType != ThingType.Pawn)
            return (def.graphicData, false);

        switch (thing.FacingDirection)
        {
            case FacingDirection.North:
            {
                var data = def.graphicBack ?? def.graphicData;
                return (data, false);
            }
            case FacingDirection.East:
            {
                var data = def.graphicSide ?? def.graphicData;
                return (data, false);
            }
            case FacingDirection.West:
            {
                var data = def.graphicSide ?? def.graphicData; // fallback-safe
                return (data, true); // keep west-flip behavior
            }
            default:
                return (def.graphicData, false);
        }
    }

    private static (GraphicData data, bool flipped) GetGraphicDataFor(FacingDirection facingDirection, CompEquipment equipment)
    {
        switch (facingDirection)
        {
            case FacingDirection.North:
                return (equipment.Props.equippedGraphic, false);
            case FacingDirection.East:
                return (equipment.Props.equippedGraphicSide, false);
            case FacingDirection.West:
                return (equipment.Props.equippedGraphic, true); // keep west-flip behavior
            default:
                return (equipment.Props.equippedGraphic, false);
        }
    }

    private void Update()
    {
        Rect camBounds = Find.Camera.CalculateCameraScreenRect();

        mpb ??= new();

        foreach (var thing in things)
        {
            Rect thingBounds  = thing.DrawBounds;

            if (!camBounds.Overlaps(thingBounds))
                continue; // cull invisible sprite
                
            DrawThing(thing, mpb, spriteMaterial);
        }
    }

    private static void DrawThing(Thing thing, MaterialPropertyBlock mpb, Material spriteMaterial)
    {            
        (GraphicData data, bool flipped) = GetGraphicDataFor(thing);
        
        if( data?.sprite == null )
            return;

        bool selected = Find.Selector.IsSelectable(thing);

        DrawSprite(data.sprite, thing.DrawPos, thing.rotation, data.scale, thing.def.size, mpb, spriteMaterial, flipped, data.brightness, 
            selected: selected);

        // draw equipped weapon
        if( thing.CompEquipmentTracker != null )
        {
            var weaponEq = thing.CompEquipmentTracker.EquippedWeapon;
            if( weaponEq != null )
            {
                Thing weapon = weaponEq.parent;

                (GraphicData weaponData, bool _) = GetGraphicDataFor(thing.FacingDirection, weaponEq);

                Vector3 weaponPos = GetEquipmentOffset(weaponEq, thing.DrawPos, thing.FacingDirection);           

                DrawSprite(weaponData.sprite, weaponPos, thing.rotation, weaponData.scale, weapon.def.size, mpb, spriteMaterial, flipped, weaponData.brightness);
            }
        }
    }

    private static Vector3 GetEquipmentOffset(CompEquipment equipment, Vector3 drawPos, FacingDirection facingDirection)
    {
        Vector3 eqOffset = Find.Camera.transform.forward * 0.0002f;

        // z offset
        drawPos += facingDirection == FacingDirection.North ? eqOffset : -eqOffset;  

        // direction
        drawPos += facingDirection switch
        {
            FacingDirection.East => new Vector3(0.15f, -0.1f, 0f),
            FacingDirection.West => new Vector3(-0.15f, -0.1f, 0f),
            _                    => Vector3.zero
        };

        return drawPos;
    }

    private static void DrawSprite(Sprite sprite, Vector3 drawPos, float rot, float scale, Vector2 size, 
        MaterialPropertyBlock mpb, Material material, bool flipped, float brightness = 1f, bool selected = false)
    {
        Mesh mesh = GetQuad(size);
        Texture2D texture = sprite.texture;
        Rect textRect = sprite.rect;

        mpb.Clear();
        mpb.SetTexture("_MainTex", texture);

        mpb.SetVector("_MainTex_ST",
            new Vector4(textRect.width / texture.width, textRect.height / texture.height,
                        textRect.x     / texture.width,  textRect.y      / texture.height));

        Quaternion rotation = Find.CameraController.Mode == CameraMode.Perspective ? 
            Quaternion.LookRotation(Find.Camera.transform.forward, Vector3.up) :
            Quaternion.Euler(0, 0, rot);

        Vector3 flipScale  = flipped ? new Vector3(-1f, 1f, 1f) : Vector3.one;
        Vector3 finalScale = Vector3.Scale(flipScale, Vector3.one * scale);

        Color color = Color.white * brightness;

        if( selected )
        {
            float pulse = Mathf.Sin(Time.time * 6f) * 0.25f + 1.25f;
            color *= pulse; // e.g. (1.25, 1.25, 1.25)

            outlineMpb ??= new();
            outlineMpb.SetTexture("_MainTex", texture);
            outlineMpb.SetColor("_Color", new Color(1f, 1f, 1f, 0.6f));

            var outlineScale = finalScale * 1.1f; // slightly larger
            var outlineMatrix = Matrix4x4.TRS(drawPos, rotation, outlineScale);
            Graphics.DrawMesh(mesh, outlineMatrix, outlineMaterial, 0, null, 0, outlineMpb);
        }
        
        mpb.SetColor("_Color", color);

        var matrix = Matrix4x4.TRS(drawPos, rotation, finalScale);
        
        Graphics.DrawMesh(mesh, matrix, material, 0, null, 0, mpb);
    }
}