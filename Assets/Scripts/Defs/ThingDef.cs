using System;
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

[Serializable]
public class GraphicData
{
    public Sprite sprite;
    public float scale = 1f;
}

[CreateAssetMenu]
public class ThingDef : Def
{
    public string label;
    public Vector2 size;
    public GraphicData graphicData;
    public Traversability traversability;
    public ThingType thingType;
    public float moveSpeed = 2f;
    
    [SerializeReference]
    public List<CompProperties> comps = new();
    
    // Pawn
    public GraphicData graphicBack;
    public GraphicData graphicSide;
}