using System;
using System.Collections.Generic;
using UnityEngine;

public static class ThingUtils
{
    public static bool IsHashInterval(this Thing thing, int interval)
    {
        return (Find.Ticker.TicksGame + HashCode.Combine(thing.UniqueId, 6734986546)) % interval == 0;
    }

    public static bool IsHashInterval(int offset, int interval)
    {
        return (Find.Ticker.TicksGame + HashCode.Combine(offset, 6734986546)) % interval == 0;
    }

    public static IEnumerable<Interaction> GetAllInteractions(this Thing thing, Thing actor )
    {
        InteractionContext context = new(actor, thing);
        
        foreach(var interaction in thing.GetInteractions(context) )
        {
            yield return interaction;
        }
    }

    public static bool IsPlayerControlled(this Thing thing)
    {
        return Find.PlayerController.IsBeingControlled(thing);
    }
}