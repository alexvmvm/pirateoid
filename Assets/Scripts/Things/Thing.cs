using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing
{
    //Const
    private static readonly Vector2[] Extents =
    {
        new (-0.5f, -0.5f),
        new (-0.5f,  0.5f),
        new ( 0.5f,  0.5f),
        new ( 0.5f, -0.5f)
    };

    //Config
    public ThingDef def;
    public Vector2 position;
    public float rotation;

    //Working vars
    private List<Vector2> corners = new();

    //Props
    public List<Vector2> Corners
    {
        get
        {
            corners.Clear();            

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Quaternion.Euler(0, 0, rotation) * new Vector2(
                    Extents[i].x * def.size.x, 
                    Extents[i].y * def.size.y);
                
                corners.Add(position + offset);
            }

            return corners;
        }
    }

    public void PostMake() {}
    public void PostSpawn() 
    {
        Find.SpriteManager.Register(this);
        Find.NavMeshManager.Register(this);
        Find.RegionManager.Notify_ThingAdded(this);
        Find.PlayerController.Notify_ThingSpawned(this);
    }

    public bool Contains(Vector2 pos) => GeomUtils.PointInPolygon(pos, Corners);

    public virtual void Move(Vector2 delta)
    {
        position += def.moveSpeed * Find.Ticker.TickInterval * delta;

        Find.RegionManager.Notify_ThingMoved(this);
        
    }
}
