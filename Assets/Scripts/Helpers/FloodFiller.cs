using System;
using System.Collections.Generic;

public static class FloodFiller
{
    /// <summary>
    /// Performs a generic flood fill starting from a root node.
    /// </summary>
    /// <typeparam name="T">Type of node (e.g. Region, CellVec, etc.)</typeparam>
    /// <param name="start">The root node to begin flood fill.</param>
    /// <param name="passCheck">Predicate to determine if a node is fillable.</param>
    /// <param name="processor">Action to apply to each node visited.</param>
    /// <param name="getNeighbors">Function to retrieve neighbors of a node.</param>
    public static void FloodFill<T>(
        T start,
        Func<T, bool> passCheck,
        Action<T> processor,
        Func<T, IEnumerable<T>> getNeighbors
    )
    {
        if (!passCheck(start))
            return;

        var open = new Queue<T>();
        var visited = new HashSet<T>();

        open.Enqueue(start);
        visited.Add(start);

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            processor(current);

            foreach (var neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor) && passCheck(neighbor))
                {
                    open.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
    }
}