using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class CompProperties_Jobs : CompProperties
{
    public CompProperties_Jobs()
	{
		compClass = typeof(CompPawnJobs);
	}
}

public class CompPawnJobs : ThingComp
{
    public CompPawnJobs(Thing parent) : base(parent)
    {
    }

    public CompPawnJobs(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    //Working vars
    protected List<Job> jobs = new();
    protected Job currentJob;

    //Props
    public Job CurJob => currentJob;
    public bool AnyJob => CurJob != null || jobs.Count > 0;
    public int GUIOrder => GUIDrawOrder.Default;

    public void StartJob(Job job)
    {
        EndCurrentJob();

        jobs.Clear();

        currentJob = job;
        currentJob.Start();
    }

    public void QueueJob(Job job)
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


#if UNITY_EDITOR
public partial class CompProperties_Jobs
{
    
}
#endif