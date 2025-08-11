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
        var thing = ThingSpawner.SpawnThing(Props.projectile, GetShootOrigin());
        var projectile = thing.GetComp<CompProjectile>();
        
        projectile.Fire(direction);
    }

    private Vector2 GetShootOrigin()
    {
        Vector3 pos = parent.DrawPos;

        pos += parent.RootOwner.FacingDirection switch
        {
            FacingDirection.East => Vector3.right * (parent.def.size.x/2f),
            FacingDirection.West => Vector3.left * (parent.def.size.x/2f),
            _                    => Vector3.zero
        };
        
        return pos;
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetShootOrigin(), 0.15f);
        Gizmos.DrawLine(parent.DrawPos, GetShootOrigin());
        Gizmos.color = Color.white;
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