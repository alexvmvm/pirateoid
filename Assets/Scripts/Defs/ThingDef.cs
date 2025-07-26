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
    public Vector2Int size;
    public Sprite sprite;
    public float scale = 1f;
    public Traversability traversability;
    public ThingType thingType;
    public float moveSpeed = 2f;
    
    // Pawn
    public bool playerControllable;
}