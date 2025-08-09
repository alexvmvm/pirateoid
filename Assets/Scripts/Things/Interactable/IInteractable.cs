using System;
using System.Collections.Generic;

public readonly struct InteractionContext
{
    public readonly Thing actor;
    public readonly Thing target;

    //Props
    public bool ActorIsPawn => actor.def.thingType == ThingType.Pawn;

    public InteractionContext(Thing actor, Thing target)
    {
        this.actor = actor;
        this.target = target;
    }
}

public record Interaction
{
    public Action action;
    public string label;
}

public interface IInteractable
{
    public IEnumerable<Interaction> GetInteractions(InteractionContext context);
}