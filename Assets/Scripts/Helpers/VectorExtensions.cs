using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Calculates the centroid (average position) of a cluster of Vector3 points.
    /// </summary>
    public static Vector2 Centroid(this IEnumerable<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (var point in points)
        {
            sum += point;
            count++;
        }

        if (count == 0)
            return Vector2.zero; // Or throw an exception, depending on your use case

        return sum / count;
    }

    public static Vector3 ToVector3(this Vector2 v) => new(v.x, v.y);
    public static Vector3 ToVector3(this Vector2Int v) => new(v.x, v.y);
    public static Vector2 ToCenter(this Vector2Int v) => new(v.x + 0.5f, v.y + 0.5f);
}