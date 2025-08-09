using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum FacingDirection
{
    South,
    East,
    North,
    West
}

[Serializable]
public partial class CompProperties_Moveable : CompProperties
{
    public CompProperties_Moveable()
	{
		compClass = typeof(CompMoveable);
	}
}

public class CompMoveable : ThingComp
{
    //Props
    public FacingDirection FacingDirection => facingDirection;

    //Working vars
    private FacingDirection facingDirection = FacingDirection.South;

    public CompMoveable(Thing parent) : base(parent)
    {
    }

    public CompMoveable(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public void Move(Vector2 delta)
    {
        var before = parent.position.ToGroundCell();

        parent.position += parent.def.moveSpeed * Find.Ticker.TickInterval * delta;

        var after = parent.position.ToGroundCell();

        Profiler.BeginSample("Thing.Move");
        if( before != after )
        {
            if( after.InBounds(Find.Map) )
                Find.RegionManager.Notify_ThingMoved(parent);
            else
            {
                if( !Find.PlayerController.IsBeingControlled(parent) )
                    parent.Destroy();
            }
        }
        Profiler.EndSample();

        if( parent.def.thingType == ThingType.Pawn )
        {
            if( delta != Vector2.zero )
            {
                if( Mathf.Abs(delta.y) > Mathf.Abs(delta.x) )
                    facingDirection = delta.y > 0 ? FacingDirection.North : FacingDirection.South;
                else
                    facingDirection = delta.x > 0 ? FacingDirection.East  : FacingDirection.West;
            }
        }
    }

    public override void Tick()
    {
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Moveable
{
    
}
#endif