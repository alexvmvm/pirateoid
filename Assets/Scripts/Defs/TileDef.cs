using UnityEngine;

[CreateAssetMenu]
public class TileDef : Def
{
    public int id;
    public string label;
    public Material material;
    public bool walkable;
    public bool renderAsWall;
}