using System;

[Serializable]
public abstract class CompProperties
{
    public Type compClass = typeof(ThingComp);

    public CompProperties(){}
	public CompProperties(Type compClass)
	{
		this.compClass = compClass;
	}
}