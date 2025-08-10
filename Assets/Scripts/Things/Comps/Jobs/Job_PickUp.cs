using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_PickUp : Job
{
    private Thing target;

    public override IEnumerable<IReservable> Reservables
    {
        get
        {
            yield break;
        }
    }

    public Job_PickUp(Thing parent, Thing target) : base(parent) 
    {
        this.target = target;
    }

    public override IEnumerable<Toil> CreateToils()
    {
        yield return Toils.MoveTo(parent, target);
        yield return Toils.PickUp(parent, target, tryEquip: true);
    }
}
