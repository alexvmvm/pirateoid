using System;

public abstract class ThingComp
{
    //Links
    public Thing parent;

    //Config
	public CompProperties props;

    protected ThingComp(Thing parent)
	{
		this.parent = parent;
	}

	public ThingComp(Thing parent, CompProperties props)
	{
		this.parent = parent;
		this.props = props;
	}

	public virtual void PostMake() 
    {
	}
	
    public virtual void Tick()
    {
        
    }

	public virtual void FrameUpdate()
	{
		
	}

	public virtual void DrawGizmos()
	{

	}
}