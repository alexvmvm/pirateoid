using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public enum SpawnState : byte
{
    Unspawned = 0,
    Spawned   = 1
}

public class Thing : ITickable, IInteractable
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
    private int id;
    private List<Vector2> corners = new();
    public List<ThingComp> comps = new();
    private SpawnState spawnState;
    
    //Comps
    private CompMoveable moveable;
    private CompPathFollower pathFollower;
    private CompPawnJobs jobs;

    //Props
    public int UniqueId => id;
    public bool Spawned => spawnState == SpawnState.Spawned;
    public CompMoveable CompMoveable => moveable;
    public CompPathFollower CompPathFollower => pathFollower;
    public CompPawnJobs CompJobs => jobs;
    public FacingDirection FacingDirection => CompMoveable != null ? CompMoveable.FacingDirection : default;
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
    public Vector3 DrawPos
    {
        get
        {
            var offset = Find.CameraController.Mode == CameraMode.Perspective ? 
                Find.Camera.transform.up * (def.size.y / 2f)  :
                new Vector3();

            return new Vector3(position.x, position.y, 0f) + offset;
        }
    }
    public Rect Bounds
    {
        get
        {
            Vector3 worldPos = new(DrawPos.x, DrawPos.y, 0f);

            // Sprite bounds in world space
            float halfWidth = def.size.x / 2f;
            float halfHeight = def.size.y / 2f;

            // Corners in world space (billboarded means flat facing camera)
            Vector3 bottomLeft = worldPos + new Vector3(-halfWidth, -halfHeight, 0);
            Vector3 topRight   = worldPos + new Vector3(halfWidth, halfHeight, 0);

            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }
    }
    public Rect DrawBounds
    {
        get
        {
            Rect bounds = Bounds;

            // Project to screen space
            Vector3 screenBL = Camera.main.WorldToScreenPoint(bounds.min);
            Vector3 screenTR = Camera.main.WorldToScreenPoint(bounds.max);

            return new Rect(
                screenBL.x,
                screenBL.y,
                screenTR.x - screenBL.x,
                screenTR.y - screenBL.y
            );

        }
    }
    // --- Ownership ---
    public CompContainer HeldContainer { get; private set; }    // null when on ground
    public Thing ThingOwner => HeldContainer?.parent;            // convenience
    public bool IsHeld => HeldContainer != null;
    // <summary>Top-most owner (e.g., pawn that owns a backpack that holds this item).</summary>
    public Thing RootOwner
    {
        get
        {
            var cur = this;
            // walk up via owner chain to the top-most Thing
            while (cur.ThingOwner != null && cur.ThingOwner != cur)
                cur = cur.ThingOwner;
            return cur;
        }
    }
    public Vector2 PositionHeld => RootOwner.position;

    public void PostMake() 
    {
        id = Find.UniqueIdManager.GetNextThingID();

        InitializeComps();

        moveable        = GetComp<CompMoveable>();
        pathFollower    = GetComp<CompPathFollower>();
        jobs            = GetComp<CompPawnJobs>();
        spawnState      = SpawnState.Unspawned;
    }

    public void Spawn() 
    {
        spawnState = SpawnState.Spawned;

        Find.SpriteManager.Register(this);
        Find.RegionManager.Notify_ThingAdded(this);
        Find.Ticker.Register(this);
    }

    public void DeSpawn()
    {
        spawnState = SpawnState.Unspawned;
        
        Find.Ticker.DeRegister(this);
        Find.SpriteManager.DeRegister(this);
        Find.RegionManager.Notify_ThingRemoved(this);
    }

    public void Destroy()
    {
        // do some stuff

        DeSpawn();
    }

    public bool Contains(Vector2 pos) => GeomUtils.PointInPolygon(pos, Corners);

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

        for( int i = 0; i < comps.Count; i++ )
            comps[i].PostMake();
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

    public IEnumerable<Interaction> GetInteractions(InteractionContext context)
    {
        // pawn picking something up
        if( def.thingType == ThingType.Item 
            && context.ActorIsPawn
            && context.actor.GetComp<CompContainer>() is CompContainer container 
            && container.CanAdd(this) )
        {
            yield return new Interaction() 
            {
                action = () => context.actor.CompJobs.StartJob(new Job_PickUp(context.actor, context.target)),
                label = "Pick up " + context.target.def.label
            };
        }

        if( !comps.NullOrEmpty() )
        {
            for(int i = 0; i < comps.Count; i++)
            {
                if( comps[i] is IInteractable interactable)
                {
                    foreach(var interaction in interactable.GetInteractions(context) )
                    {
                        yield return interaction;
                    }
                }
            }
        }
    }

    internal void SetOwner(CompContainer container)
    {
        HeldContainer = container;
    }

    internal void ClearOwner()
    {
        HeldContainer = null;
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
