using System.Linq;
using UnityEngine;

public static class Toils
{
    public static Toil MoveTo(Thing pawn, Vector2Int cell, PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        var toil = new Toil
        {
            init = () => pawn.CompPathFollower.MoveTo(cell, pathEndMode)
        };

        toil.tick = () => 
        {
            if( !pawn.CompPathFollower.Pathing )
                toil.Complete();
        };

        return toil;
    }

    public static Toil MoveTo(Thing pawn, Thing thing, PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        var toil = new Toil
        {
            init = () => pawn.CompPathFollower.MoveTo(thing, pathEndMode)
        };

        toil.tick = () => 
        {
            if( !pawn.CompPathFollower.Pathing )
                toil.Complete();
        };

        return toil;
    }

    public static Toil Wait(int ticks)
    {
        var toil = new Toil();

        toil.init = () => toil.waitTicks = ticks;
        toil.tick = () =>
        {
            toil.waitTicks--;

            if( toil.waitTicks <= 0 )
                toil.Complete();
        };

        return toil;
    }

    public static Toil PickUp(Thing pawn, Thing item, bool tryEquip = false)
    {
        var toil = new Toil();
        
        toil.init = () => 
        {
            var container = pawn.GetComp<CompContainer>();
            if( container != null )
                container.Add(item);
            else
                Debug.Log($"Tried to pick up {item} wihtout a way to store it.");

            if( tryEquip )
            {
                var equipment = pawn.GetComp<CompEquipmentTracker>();
                if( equipment != null && equipment.CanEquip(item) )
                    equipment.Equip(item);
            }
            
            toil.Complete();
        };

        return toil;
    }

    // public static Toil PlaceInContainer(Pawn pawn, Item item, Container container)
    // {
    //     var toil = new Toil();
        
    //     toil.init = () => 
    //     {
    //         if( item.ParentHolder != null )
    //             item.RemoveFromHolder();

    //         item.AddToHolder(container);
    //         toil.Complete();

    //         Find.SoundManager.Notify_ItemAddedToContainer(container.transform.position);
    //     };

    //     return toil;
    // }

    // public static Toil PlaceInCell(Item item, Vector2Int cell)
    // {
    //     var toil = new Toil();
        
    //     toil.init = () => 
    //     {
    //         if( item.ParentHolder != null )
    //             item.RemoveFromHolder();

    //         item.Cell = cell;

    //         var storage = cell.GetStorageZone();
    //         if( storage != null )
    //         {
    //             if( storage.Allows(item.def) )
    //                 item.AddToHolder(storage);
    //             else
    //                 Debug.LogWarning("Placed item in cell containing a storage zone that doesn't allow this thing " + item + ".");
    //         }
    //         toil.Complete();

    //         Find.SoundManager.Notify_ItemPlacedInCell(cell.ToVector2());
    //     };

    //     return toil;
    // }
}