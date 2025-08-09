using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ToilState
{
    Running,
    Completed,
    Failed
}

public interface IJobber 
{
    GameObject ReservationRoot { get; }
}

public class Toil
{
    //Working vars 
    public Action tick = () => {};
    public Action init = () => {};
    public int waitTicks;
    public int initTick = -1;
    public int progressTicks = 0;
    private List<Action> preTickActions;
    private List<Action> onGUIActions;
    private List<Func<bool>> failConditions;
    private List<Func<bool>> skipConditions;
    private List<Action> completeActions;

    // Auto-property with private setter for ChoreState
    public ToilState State { get; private set; } = ToilState.Running;
    public bool ShouldSkip
    {
        get
        {
            if( skipConditions.NullOrEmpty() )
                return false;

            for(var i = 0; i < skipConditions.Count; i++)
            {
                if( skipConditions[i]() )
                    return true;
            }

            return false;
        }
    }

    // Reset method using expression-bodied member
    public void Reset() => State = ToilState.Running;

    // Complete method with streamlined syntax
    public void Complete()
    {
        if (State == ToilState.Failed)
        {
            Debug.LogError($"Tried to complete a failed chore.");
            return;
        }

        if( !completeActions.NullOrEmpty() )
        {
            for(var i = 0; i < completeActions.Count; i++)
            {
                completeActions[i]();
            }
        }

        State = ToilState.Completed;
    }

    // Fail method with streamlined syntax
    public void Fail()
    {
        if (State == ToilState.Completed)
        {
            Debug.LogError($"Tried to fail a completed chore.");
            return;
        }

        State = ToilState.Failed;
    }

    public void AddPreTickAction(Action action)
    {
        preTickActions ??= new();
        preTickActions.Add(action);
    }

    public void AddOnGUIAction(Action action)
    {
        onGUIActions ??= new();
        onGUIActions.Add(action);
    }

    public void AddFailCondition(Func<bool> failCondition)
    {
        failConditions ??= new();
        failConditions.Add(failCondition);
    }

    public void AddSkipCondition(Func<bool> skipCondition)
    {
        skipConditions ??= new();
        skipConditions.Add(skipCondition);
    }

    public void AddCompleteAction(Action action)
    {
        completeActions ??= new();
        completeActions.Add(action);
    }

    public virtual void DoGUI()
    {
        if( !onGUIActions.NullOrEmpty() )
        {
            for(var i = 0; i < onGUIActions.Count; i++)
            {
                onGUIActions[i]();
            }
        }
    }

    public virtual void Tick()
    {
        if( init == null && tick == null )
        {
            Debug.LogError("Toil started with not init or tick method. Failing.");
            State = ToilState.Failed;
            return;
        }

        if (initTick < 0)
        {
            initTick = Find.Ticker.TicksGame;
            
            if( init != null )
                init();
        }

        if( tick != null )
        {
            if( !failConditions.NullOrEmpty() )
            {
                for(var i = 0; i < preTickActions.Count; i++)
                {
                    if(failConditions[i]())
                    {
                        State = ToilState.Failed;
                        return;
                    }
                }
            }

            if( !preTickActions.NullOrEmpty() )
            {
                for(var i = 0; i < preTickActions.Count; i++)
                {
                    preTickActions[i]();
                }
            }

            tick();   
        }
        else
            State = ToilState.Completed;
    }
}

public enum JobState
{
    Ready,
    Running,
    Failed,
    Completed
}

public abstract class Job
{
    //Working vars 
    private readonly List<Toil> toils = new();
    private int toilIndex = 0;
    private JobState state = JobState.Ready;
    private int waitTicks;
    private int initTick = -1;
    private int progressTicks = 0;
    public bool interruptible = false;
    private List<Func<bool>> failConditions;
    protected Thing parent;

    //Props
    public JobState State  
    {
        get => state;
        set => state = value;
    }
    public Toil CurrentToil => toils[toilIndex];
    public virtual IEnumerable<IReservable> Reservables { get; } = Enumerable.Empty<IReservable>();

    public Job(Thing parent)
    {   
        this.parent = parent;
    }

    public abstract IEnumerable<Toil> CreateToils();

    public virtual void Start()
    {
        toilIndex = 0;
        failConditions = null;
        
        toils.Clear();
        toils.AddRange(CreateToils());

        MakeReservations();

        State = JobState.Running;
    }

    public void AddFailCondition(Func<bool> failCondition)
    {
        failConditions ??= new();
        failConditions.Add(failCondition);
    }

    private void MakeReservations()
    {
        foreach(var reservable in Reservables)
        {
            if( !Find.Reservations.Reserve(reservable, parent) )
            {
                Debug.LogError("Failed to reserve " + reservable);
                Fail();
            }
        }
    }

    private void ClearReservations()
    {
        foreach(var reservable in Reservables)
        {
            Find.Reservations.Unreserve(reservable, parent);
        }
    }

    public virtual void OnCompleted() {}
    public virtual void OnFailed() {}

    public virtual void Notify_ThingDestroyed(Thing thing) {}

    public void Complete()
    {
        State = JobState.Completed;

        toilIndex = 0;

        ClearReservations();

        OnCompleted();
    }

    public void Fail()
    {
        State = JobState.Failed;

        toilIndex = 0;

        ClearReservations();

        OnFailed();
    }

    public void DoGUI()
    {
        if( State != JobState.Running )
            return;
        
        var toil = CurrentToil;

        toil.DoGUI();
    }

    public bool IsContinuation(Job other)
    {
        return GetType() == other.GetType();
    }

    public void Tick()
    {
        if( State != JobState.Running )
            return;

        //Check if we should skip any toils
        while(CurrentToil.ShouldSkip)
            toilIndex++;

        if( toilIndex >= toils.Count )
        {
            Complete();
            return;
        }

        var toil = CurrentToil;

        if( !failConditions.NullOrEmpty() )
        {
            for(var i = 0; i < failConditions.Count; i++)
            {
                if( failConditions[i]() )
                {
                    Fail();
                    return;
                }
            }
        }

        toil.Tick();

        switch(toil.State)
        {
            case ToilState.Completed:
            {
                toilIndex++;
                
                if( toilIndex >= toils.Count )
                    Complete();
            }
            break;
            case ToilState.Failed:
            {
                State = JobState.Failed;
            }
            break;
        }
    }

    /* Notify */
    public virtual void Notify_ThingAdded(Thing thing) {}

    public override string ToString()
    {
        return $"{base.ToString()} ({toilIndex+1}/{toils.Count})";
    }

    // public virtual void SaveLoad()
    // {
    //     Persist.Value(ref toilIndex, "toilIndex");
    //     Persist.Value(ref state, "state");

    //     if( Persist.mode == PersistMode.Saving && State == JobState.Running )
    //     {
    //         var currentToil = CurrentToil;
    //         if( currentToil != null )
    //         {
    //             waitTicks = Persist.Value(currentToil.waitTicks, "waitTicks");
    //             initTick = Persist.Value(currentToil.initTick, "initTick");
    //             progressTicks = Persist.Value(currentToil.progressTicks, "progressTicks");
    //         }
    //     }
    //     else if( Persist.mode == PersistMode.Loading )
    //     {
    //         Persist.Value(ref waitTicks, "waitTicks");
    //         Persist.Value(ref initTick, "initTick");
    //         Persist.Value(ref progressTicks, "progressTicks");
    //     }
        
    //     if( Persist.mode == PersistMode.PostLoad )
    //     {
    //         toils.Clear();
    //         toils.AddRange(CreateToils());
            
    //         MakeReservations();
            
    //         var currentToil = CurrentToil;
    //         if( currentToil != null )
    //         {
    //             currentToil.initTick = initTick;
    //             currentToil.waitTicks = waitTicks;
    //             currentToil.progressTicks = progressTicks;
    //         }
    //     }
    // }
}
