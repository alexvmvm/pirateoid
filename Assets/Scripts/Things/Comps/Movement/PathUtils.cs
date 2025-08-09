using UnityEngine;
using System.Collections.Generic;
using System;

public enum PathEndMode
{
    OnCell,
    NextTo
}

public static class PathUtils
{
    private static readonly PriorityQueue<float, Vector2Int> frontier = new();
    private static readonly Dictionary<Vector2Int, Vector2Int> cameFrom = new();
    private static readonly Dictionary<Vector2Int, float> costSoFar = new();

    public static bool TryFindPath(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> predicate, ref List<Vector2Int> path, PathEndMode pathEndMode = PathEndMode.OnCell)
    {
        if( !predicate(start) )
            return false;

        if( pathEndMode == PathEndMode.OnCell && !predicate(end) )
            return false;

        IEnumerable<Vector2Int> GetNeighbours(Vector2Int p)
        {
            for(var i = 0; i < 4; i++)
            {
                Vector2Int c = p + Cells.Cardinal[i];

                if( !c.InBounds(Find.Map) || !predicate(c) )
                    continue;
                
                yield return c;
            }
        }

        // Compute the path
        return TryFindPathAStar(
            start, 
            end, 
            (p) => pathEndMode switch
            {
                PathEndMode.OnCell  => p.Equals(end),
                PathEndMode.NextTo  => p.AdjacentTo(end),
                _                   => false,
            },
            ref path, 
            (a, b) => Vector2.Distance(a, b), // Heuristic 
            (from, to) => to.PathCost(),
            (c) => GetNeighbours(c)
        );
    }
    
    private static bool TryFindPathAStar(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> endCheck, ref List<Vector2Int> path, Func<Vector2Int, Vector2Int, float> heuristic, Func<Vector2Int, Vector2Int, float> graphCost,  Func<Vector2Int, IEnumerable<Vector2Int>> neighbours)
    {
        return TryFindPathAStar(start, endCheck, ref path, (current) => heuristic(current, end), graphCost, neighbours);
    }

    private static bool TryFindPathAStar(Vector2Int start, Func<Vector2Int, bool> end, ref List<Vector2Int> path, Func<Vector2Int, float> heuristic, Func<Vector2Int, Vector2Int, float> graphCost,  Func<Vector2Int, IEnumerable<Vector2Int>> neighbours)
    {
        if(end(start))
        {
            path.Add(start);
            return true;
        }

        frontier.Clear();
        cameFrom.Clear();
        costSoFar.Clear();

        frontier.Add(0f, start);
        cameFrom[start] = start;
        costSoFar[start] = 0f;
        
        while(frontier.Count > 0)
        {
            var current = frontier.DeQueue();
            if(end(current)) 
            {
                while(!current.Equals(start))
                {
                    path.Add(current);
                    current = cameFrom[current];       
                }

                path.Reverse();
                return true;
            }

            foreach(var next in neighbours(current))
            {
                var cost = costSoFar[current] + graphCost(current, next);
                if(!costSoFar.ContainsKey(next) || cost < costSoFar[current])
                {
                    costSoFar[next] = cost;
                    var priority = cost + heuristic(next);
                    frontier.Add(priority, next);
                    cameFrom[next] = current;
                }
            }
        }
        
        return false;
    }
}