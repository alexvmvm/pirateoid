using UnityEngine;

public class PlayerController : MonoBehaviour, ITickable, IGUI
{
    public CompControllable controllable;

    //Props
    public Thing ControlledThing => controllable?.parent;
    public int GUIOrder => UIOrder.Inventory;

    void OnEnable()
    {
        Find.Ticker.Register(this);
        Find.GUIHandler.Register(this);
    }

    void OnDisable()
    {
        Find.Ticker.DeRegister(this);
        Find.GUIHandler.Register(this);
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

    public void DoGUI()
    {
        const float Height = 35;

        var x = UI.Gap;
        var y = UIScreen.Height - Height - UI.GapLarge;
        Rect rect = new Rect(x, y, UIScreen.Width - 2 * UI.Gap, Height);

        InventoryUIUtils.DrawInventory(rect, controllable.parent);
    }
}
