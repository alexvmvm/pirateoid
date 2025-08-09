using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class CompProperties_PathFollower : CompProperties
{
    //Config
    public float acceleration = 2f; // How quickly the pawn accelerates
    public float decelerationDistance = 2f; // The distance from the target at which the pawn should start decelerating
    public float stoppingDistance = 0.1f; // The distance at which the pawn stops moving

    public CompProperties_PathFollower()
	{
		compClass = typeof(CompPathFollower);
	}
}

public class CompPathFollower : ThingComp
{
    //Working vars
    private List<Vector2> exactPath = new(); // List of waypoints for the path
    private List<Vector2Int> waypoints = new();
    private int currentWaypointIndex = -1;
    private Vector2 currentVelocity;
    private Vector2Int currentGroundCell;
    private PathFinderRequest request;

    //Props
    public CompProperties_PathFollower Props => (CompProperties_PathFollower)props;
    public bool Pathing => currentWaypointIndex > 0 || (request != null && request.state != PathState.Failed); 
    public bool ReachedEndOfPath => currentWaypointIndex >= exactPath.Count;

    public CompPathFollower(Thing parent) : base(parent)
    {
    }

    public CompPathFollower(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public void MoveTo(Vector2 pos, PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        if( request != null )
            Debug.LogWarning("Tried to start a new path whilst calculating an existing path.");

        waypoints.Clear();
        request = Find.PathFinder.FindPath(parent.position.ToGroundCell(), pos.ToGroundCell(), ref waypoints, pathEndMode);     
        FollowPath(request);   
    }

    public void MoveTo(Thing target, PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        if( request != null )
            Debug.LogWarning("Tried to start a new path whilst calculating an existing path.");

        waypoints.Clear();
        request = Find.PathFinder.FindPath(parent.position.ToGroundCell(), target, ref waypoints, pathEndMode);
        FollowPath(request);          
    }
    
    // You can call this method to set a new path for the pawn
    private void FollowPath(PathFinderRequest request)
    {
        if( request.state == PathState.Calculating )
            return;
        
        if( request.state != PathState.Success )
        {
            Debug.Log("Tried to follow a path that hasn't been calculated yet.");
            return;
        }

        SimplifyPath(request.path, ref exactPath, request.target);

        currentWaypointIndex = 0;
    }

    private void SimplifyPath(List<Vector2Int> inPath, ref List<Vector2> outPath, Thing target)
    {
        outPath.Clear();
        outPath.Add(parent.position);
        
        foreach (var point in inPath)
        {
            outPath.Add(point.Center());
        }

        // add end if one exists
        if( target != null )
            outPath.Add(target.position);

        //simplify
        Vector2 start = outPath[0];
        Vector2 end   = outPath[^1];

        outPath.Clear();
        outPath.Add(start);
        outPath.Add(end);
    }

    public void Stop()
    {
        currentWaypointIndex = -1;
        exactPath.Clear();
        currentVelocity = Vector2.zero;

        if( request != null )
        {
            Find.PathFinder.Stop(request);
            request = null;
        }
    }

    private void Notify_MovedIntoNewCell(Vector2Int prev, Vector2Int cur)
    {
        
    }

    void MoveTowardsCurrentWaypoint()
    {
        if (request != null)
        {
            if (request.state == PathState.Calculating)
                return;
            else
            {
                if (request.state == PathState.Success)
                {
                    FollowPath(request);
                    request = null;
                }
                else
                {
                    request = null;
                    Stop();
                    return;
                }
            }
        }

        if( currentWaypointIndex < 0 )
            return;

        if (ReachedEndOfPath)
        {
            Stop();
            return;
        }

        // Get the current target waypoint
        var target = exactPath[currentWaypointIndex];

        // Calculate the distance to the current target
        var distanceToTarget = Vector2.Distance(parent.position, target);

        if (distanceToTarget > Props.stoppingDistance)
        {
            var directionToTarget = (target - parent.position).normalized;

            // Move at a fixed max speed
            currentVelocity = directionToTarget;

            // Move the pawn
            parent.CompMoveable.Move(currentVelocity);
        }
        else
        {
            // Move to the next waypoint when the current one is reached
            currentWaypointIndex++;
        }

        CheckMovedToNewCell();
    }

    void CheckMovedToNewCell()
    {
        //Notify if we moved into a new cell
        var cell = parent.position.ToGroundCell();
        if( cell != currentGroundCell )
        {
            Notify_MovedIntoNewCell(currentGroundCell, cell);

            currentGroundCell = cell;
        }
    }

    void CheckTeleportToTraversableCell()
    {  
        if( !parent.Spawned || Pathing )
            return;
        
        // var currentCell = parent.position.ToGroundCell();
        // if( pawn.CanTraverse(currentCell) )
        //     return;

        //Stop();

        // FloodFiller.FloodFill(currentCell, 
        //     (p) => true, 
        //     (p) => 
        //     {
        //         if( pawn.CanTraverse(p))
        //         {
        //             transform.position = p.Center();
        //             CheckMovedToNewCell();
        //             return true;
        //         }

        //         return false;
        //     });
    }

    public override void Tick()
    {
        CheckTeleportToTraversableCell();
        MoveTowardsCurrentWaypoint();
    }
}

#if UNITY_EDITOR
public partial class CompProperties_PathFollower
{
    
}
#endif