using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class CompProperties_Controllable : CompProperties
{
    public CompProperties_Controllable()
	{
		compClass = typeof(CompControllable);
	}
}

public class CompControllable : ThingComp
{
    private bool controlled = false;
    
    private CompMoveable moveable;
    private CompPathFollower pathFollower;
    private CompPawnJobs jobs;
    private CompEquipmentTracker equipment;

    public CompControllable(Thing parent) : base(parent)
    {
    }

    public CompControllable(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public override void PostMake()
    {
        base.PostMake();

        moveable     = parent.GetComp<CompMoveable>();
        pathFollower = parent.GetComp<CompPathFollower>();
        jobs         = parent.GetComp<CompPawnJobs>();
        equipment    = parent.GetComp<CompEquipmentTracker>();
    }

    public void SetControlled(bool control)
    {
        if( controlled != control )
        {
            controlled = control;
            
            Find.PlayerController.Set(controlled ? this : null);
        }
    }

    public void HandleTickInput()
    {
        if( moveable != null )
        {
            Vector2 move = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) move += Vector2.up;
            if (Input.GetKey(KeyCode.S)) move += Vector2.down;
            if (Input.GetKey(KeyCode.A)) move += Vector2.left;
            if (Input.GetKey(KeyCode.D)) move += Vector2.right;

            if (move != Vector2.zero)
            {
                if( pathFollower != null )
                    pathFollower.Stop();

                if( jobs != null )
                    jobs.EndCurrentJob(failed: true);
                
                moveable.Move(move);
            }
        }
    }

    public void HandleFrameInput()
    {
        if( Input.GetKeyDown(KeyCode.Mouse0) )
        {
            if( equipment != null )
            {
                var weapon = equipment.EquippedWeapon;
                if( weapon != null )
                {   
                    var shooter = weapon.GetComp<CompShooter>();
                    if( shooter != null )
                    {
                        if( RaycastUtils.UIToMapPosition(Input.mousePosition, out Vector3 clickPos) )
                        {
                            var dir = ((Vector2)clickPos - parent.PositionHeld).normalized;

                            shooter.ShootInDirection(dir);
                        }
                    }
                }       
            }  
        }
    }

    public override void Tick()
    {
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Controllable
{
    
}
#endif