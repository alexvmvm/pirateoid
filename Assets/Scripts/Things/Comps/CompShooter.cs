using System;
using UnityEngine;

[Serializable]
public partial class CompProperties_Shooter : CompProperties
{
    public ThingDef projectile;
    public float speed = 1f;

    public CompProperties_Shooter()
	{
		compClass = typeof(CompShooter);
	}
}

public class CompShooter : ThingComp
{
    public CompProperties_Shooter Props => (CompProperties_Shooter)props;

    public CompShooter(Thing parent) : base(parent)
    {
    }

    public CompShooter(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public void ShootInDirection(Vector2 direction)
    {        
        var thing = ThingSpawner.SpawnThing(Props.projectile, parent.PositionHeld);
        var projectile = thing.GetComp<CompProjectile>();
        
        projectile.Fire(direction);
    }

    public override void Tick()
    {
        // if( !parent.IsPlayerControlled() && parent.IsHashInterval(120) )
        //     ShootInDirection(Vector2.down);
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Shooter
{
    public override void DrawEditorFields()
    {
        projectile  = (ThingDef)UnityEditor.EditorGUILayout.ObjectField("Projectile", projectile, typeof(ThingDef), false);
        speed       = UnityEditor.EditorGUILayout.FloatField("Projectile", speed);
    }
}
#endif