using UnityEngine;

public enum Traversability
{
    Passable,
    Impassable
}

[CreateAssetMenu]
public class ThingDef : Def
{
    public string label;
    public Vector2Int size;
    public Sprite sprite;
    public float scale = 1f;
    public Traversability traversability;
}