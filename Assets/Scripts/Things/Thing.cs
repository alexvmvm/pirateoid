using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Thing : ITickable
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
    public List<ThingComp> comps = new();

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

    public void PostMake() 
    {
        InitializeComps();
    }
    public void PostSpawn() 
    {
        Find.SpriteManager.Register(this);
        Find.NavMeshManager.Register(this);
        Find.RegionManager.Notify_ThingAdded(this);
        Find.PlayerController.Notify_ThingSpawned(this);

        Find.Ticker.Register(this);
    }

    public void PostDeSpawn()
    {
        Find.Ticker.DeRegister(this);
    }

    public bool Contains(Vector2 pos) => GeomUtils.PointInPolygon(pos, Corners);

    public virtual void Move(Vector2 delta)
    {
        var before = position.ToGroundCell();

        position += def.moveSpeed * Find.Ticker.TickInterval * delta;

        if( before != position.ToGroundCell() )
            Find.RegionManager.Notify_ThingMoved(this);
    }

    public T GetComp<T>() where T : ThingComp
    {
        for (int i = 0; i < comps.Count; i++)
        {
            if (comps[i] is T match)
                return match;
        }

        return null;
    }

    public void InitializeComps()
	{
		if (!def.comps.Any())
			return;

		comps = new List<ThingComp>();

		for( int i = 0; i < def.comps.Count; i++ )
			CreateAndAppendComponent(def.comps[i], comps);
	}
	
	private void CreateAndAppendComponent(CompProperties properties, List<ThingComp> container)
	{
		ThingComp newComp = null;
		try
		{
			newComp = (ThingComp)Activator.CreateInstance(properties.compClass, new object[] {this, properties});
		}
		catch( Exception e )
		{
			Debug.LogError($"Could not create {nameof(ThingComp)} of type {properties.compClass} for {this} using Def {def}: {e}");
		}

		if (newComp != null)	
			container.Add(newComp);
	}

    public void Tick()
    {
        Profiler.BeginSample("Thing.Tick");

        if( !comps.NullOrEmpty() )
        {
            for(var i = 0; i < comps.Count; i++)
            {
                comps[i].Tick();
            }
        }

        Profiler.EndSample();
    }
}
