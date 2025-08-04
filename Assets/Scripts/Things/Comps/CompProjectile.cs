using System;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public partial class CompProperties_Projectile : CompProperties
{
    public float damage = 1f;

    public CompProperties_Projectile()
	{
		compClass = typeof(CompProjectile);
	}
}

public class CompProjectile : ThingComp
{
    private Vector2 direction;
    private CompMoveable moveable;

    public CompProjectile(Thing parent) : base(parent)
    {
    }

    public CompProjectile(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public override void PostMake()
    {
        base.PostMake();

        moveable = parent.GetComp<CompMoveable>();
    }

    public void Fire(Vector2 direction)
    {
        this.direction = direction;
    }

    public override void Tick()
    {
        Profiler.BeginSample("CompProjectile.Tick");
        moveable.Move(direction); 
        Profiler.EndSample();
    }
}

#if UNITY_EDITOR
public partial class CompProperties_Projectile
{
    public override void DrawEditorFields()
    {
        damage = UnityEditor.EditorGUILayout.FloatField("Damage", damage);
    }
}
#endif