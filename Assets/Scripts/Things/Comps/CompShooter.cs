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

    public override void Tick()
    {
        if( Find.Ticker.TicksGame % 120 == 0 )
        {
            var thing = ThingSpawner.SpawnThing(Props.projectile, parent.position);
            var projectile = thing.GetComp<CompProjectile>();
            
            projectile.Fire(Vector2.down);
        }
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