using System.Collections.Generic;
using UnityEngine;

public abstract class JobTracker<IJobber, J> : ThingComp, IGUI, ITickable where J : Job
{
    //Working vars
    protected List<J> jobs = new();
    protected J currentJob;

    protected JobTracker(Thing parent) : base(parent)
    {
    }

    //Props
    public J CurJob => currentJob;
    public bool AnyJob => CurJob != null || jobs.Count > 0;
    public int GUIOrder => GUIDrawOrder.Default;

    void OnEnable()
    {
        Find.Ticker.Register(this);
        Find.GUIHandler.Register(this);
    }

    void OnDisable()
    {
        Find.Ticker?.DeRegister(this);
        Find.GUIHandler.DeRegister(this);
    }

    public void StartJob(J job)
    {
        EndCurrentJob();

        jobs.Clear();

        currentJob = job;
        currentJob.Start();
    }

    public void QueueJob(J job)
    {
        if( currentJob == null )
        {
            if( jobs.Count <= 0 )
                StartJob(job);
            else
            {
                StartJob(jobs[0]);
                jobs.RemoveAt(0);
                jobs.Add(job);
            }
        }
        else
            jobs.Add(job);
    }

    public void EndCurrentJob(bool failed = false)
    {
        if( currentJob != null )
        {
            if( failed )
                currentJob.Fail();
            else
                currentJob.Complete();
            
            currentJob = null;
        }
    }

    public void DoGUI()
    {
        if( currentJob != null )
        {
            currentJob.DoGUI();
        }
    }

    public override void Tick()
    {
        base.Tick();
        
        if( currentJob != null )
        {
            switch( currentJob.State )
            {
                case JobState.Completed:
                    EndCurrentJob();
                break;
                case JobState.Failed:
                    EndCurrentJob(failed: true);
                break;
                case JobState.Running:
                    currentJob.Tick();
                break;
                default:
                    Debug.LogError("Unknown job state " + currentJob.State);
                break;
            }
        }

        if( currentJob == null && jobs.Count > 0 )
        {
            currentJob = jobs[0];
            currentJob.Start();

            jobs.RemoveAt(0);
        }
    }
}