using System.Collections.Generic;
using UnityEngine;

public enum Traversability
{
    Passable,
    Impassable
}

public enum ThingType
{
    Item,
    Building,
    Pawn
}

[CreateAssetMenu]
public class ThingDef : Def
{
    public string label;
    public Vector2 size;
    public Sprite sprite;
    public float scale = 1f;
    public Traversability traversability;
    public ThingType thingType;
    public float moveSpeed = 2f;
    
    [SerializeReference]
    public List<CompProperties> comps = new();
    
    // Pawn
    public bool playerControllable;
    public Sprite spriteBack;
    public Sprite spriteSide;
}