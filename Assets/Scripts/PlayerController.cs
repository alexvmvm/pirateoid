using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ITickable
{
    public Thing current;

    void OnEnable()
    {
        Find.Ticker.Register(this);
    }

    void OnDisable()
    {
        Find.Ticker.DeRegister(this);
    }

    public void SetThing(Thing thing)
    {
        this.current = thing;

        Find.CameraController.Follow(thing);
    }

    public void Notify_ThingSpawned(Thing thing)
    {
        if( current != null || thing.def.thingType != ThingType.Pawn || !thing.def.playerControllable )
            return;

        SetThing(thing);
    }

    public void Tick()
    {
        if( current != null )
            HandlePlayerInput();   
    }

    public void HandlePlayerInput()
    {
        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;
        if (Input.GetKey(KeyCode.A)) move += Vector2.left;
        if (Input.GetKey(KeyCode.D)) move += Vector2.right;

        if (move != Vector2.zero)
        {
            current.Move(move);
        }
    }
}
