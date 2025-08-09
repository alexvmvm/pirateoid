using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathState
{
    Calculating,
    Success,
    Failed
}

public class PathFinderRequest
{
    public Vector2Int start;
    public Vector2Int end;
    public Thing target;
    public PathState state = PathState.Calculating;
    public PathEndMode pathEndMode = PathEndMode.OnCell;
    public List<Vector2Int> path;

    public PathFinderRequest(Vector2Int start, Vector2Int end, ref List<Vector2Int> path)
    {
        this.start  = start;
        this.end    = end;
        this.path   = path;
    }
}

public class PathFinder : MonoBehaviour, ITickable
{
    //Const
    private const int MaxPatherRequestsPerFrame = 5;

    //Working vars
    private readonly List<PathFinderRequest> requests = new();

    void OnEnable()
    {
        Find.Ticker.Register(this);
    }

    void OnDisable()
    {
        Find.Ticker.DeRegister(this);
    }

    public PathFinderRequest FindPath(Vector2Int start, Vector2Int end, ref List<Vector2Int> path, 
        PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        var result = new PathFinderRequest(start, end, ref path)
        {
            state = PathState.Calculating,
            pathEndMode = pathEndMode
        };
        requests.Add(result);
        return result;
    }

    public PathFinderRequest FindPath(Vector2Int start, Thing target, ref List<Vector2Int> path, 
        PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        var result = new PathFinderRequest(start, target.position.ToGroundCell(), ref path)
        {
            target = target,
            state = PathState.Calculating,
            pathEndMode = pathEndMode
        };
        requests.Add(result);
        return result;
    }

    private void Calculate(PathFinderRequest request)
    {
        request.path.Clear();
        
        if( !PathUtils.TryFindPath(request.start, request.end, c => true, ref request.path, request.pathEndMode) )
        {
            request.state = PathState.Failed;
            Debug.LogError($"Tried to path from {request.start} to {request.end} but no path could be found.");
            return;
        }

        request.state = PathState.Success;
    }

    public void Stop(PathFinderRequest request)
    {
        requests.Remove(request);
    }

    public void Tick()
    {        
        if( requests.Count > 0 )
        {
            var countToProcess = Math.Min(requests.Count, MaxPatherRequestsPerFrame);
            
            for(var i = countToProcess - 1; i >= 0; i--)
            {
                var request = requests[i];
                Calculate(request);
                requests.RemoveAt(i);
            }
        }
            
    }
}
