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

    #if UNITY_EDITOR
	public virtual void DrawEditorFields()
    {
        // Base class can draw common fields, or just be empty
    }
    #endif
}