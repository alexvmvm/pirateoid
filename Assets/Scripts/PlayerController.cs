using UnityEngine;

public class PlayerController : MonoBehaviour, ITickable
{
    public CompControllable controllable;

    //Props
    public Thing ControlledThing => controllable?.parent;

    void OnEnable()
    {
        Find.Ticker.Register(this);
    }

    void OnDisable()
    {
        Find.Ticker.DeRegister(this);
    }

    public bool IsControlled(Thing thing)
    {
        return controllable?.parent == thing;
    }

    public void Set(CompControllable controllable)
    {
        this.controllable = controllable;

        Find.CameraController.Follow(controllable.parent);
    }

    public bool IsBeingControlled(Thing thing)
        => controllable?.parent != null && controllable.parent == thing;

    public void Tick()
    {
        if( controllable != null )
            controllable.HandleTickInput();   
    }

    void Update()
    {
        if( controllable != null )
            controllable.HandleFrameInput(); 
    }
}
