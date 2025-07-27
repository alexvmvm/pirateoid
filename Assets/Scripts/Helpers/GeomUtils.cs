using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tests whether two segments AB and CD intersect.
/// Returns true if they cross (including touching endpoints),
/// and outputs the intersection point when there is a single one.
/// Colinear‑overlap returns false but can be detected via 'colinear' out flag.
/// </summary>
public static class GeomUtils
{
    public static bool LinesIntersect(
        Vector2 a, Vector2 b,
        Vector2 c, Vector2 d,
        out Vector2 intersection,
        out bool colinear)
    {
        intersection = default;
        colinear = false;

        Vector2 r = b - a;
        Vector2 s = d - c;

        float rxs = Cross(r, s);
        float qpxr = Cross(c - a, r);

        const float EPS = 1e-6f;

        // 1. Parallel (including colinear) ⇢ no single intersection point
        if (Mathf.Abs(rxs) < EPS)
        {
            // Colinear if (c‑a) × r == 0
            colinear = Mathf.Abs(qpxr) < EPS;
            return false;        // segments may overlap: handle separately if needed
        }

        // 2. Solve a + t r = c + u s
        float t = Cross(c - a, s) / rxs;
        float u = qpxr / rxs;

        // 3. True if 0 ≤ t,u ≤ 1  (within the segment bounds)
        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            intersection = a + t * r;
            return true;
        }

        return false; // lines cross outside the segment extents
    }

    private static float Cross(Vector2 v, Vector2 w) => v.x * w.y - v.y * w.x;

    public static bool PointInPolygon(Vector2 point, List<Vector2> polygon)
    {
        int crossings = 0;

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count];

            // Ensure a.y <= b.y
            if (a.y > b.y)
            {
                (a, b) = (b, a);
            }

            // Skip if point is outside vertical bounds
            if (point.y <= a.y || point.y > b.y)
                continue;

            // Compute intersection of edge with horizontal ray at point.y
            float t = (point.y - a.y) / (b.y - a.y); // 0 to 1
            float x = Mathf.Lerp(a.x, b.x, t);

            if (x > point.x)
                crossings++;
        }

        return (crossings % 2) == 1;
    }

    /// <summary>
    /// Returns the closest point on the polygon perimeter to the given point.
    /// Polygon is assumed to be closed and defined in order (CW or CCW).
    /// </summary>
    public static Vector2 ClosestPointOnPolygon(Vector2 point, List<Vector2> polygon)
    {
        Vector2 closestPoint = default;
        float minDistSq = float.MaxValue;

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count]; // wrap around to close polygon

            Vector2 candidate = ClosestPointOnSegment(point, a, b);
            float distSq = (candidate - point).sqrMagnitude;

            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closestPoint = candidate;
            }
        }

        return closestPoint;
    }

    /// <summary>
    /// Returns the closest point to 'p' on the line segment AB.
    /// </summary>
    public static Vector2 ClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float abLengthSq = ab.sqrMagnitude;
        if (abLengthSq == 0f) return a;

        float t = Vector2.Dot(p - a, ab) / abLengthSq;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    public static bool RayPlaneIntersection(Vector3 rayOrigin, Vector3 rayDir, Vector3 planePoint, Vector3 planeNormal, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;

        float denom = Vector3.Dot(planeNormal, rayDir);
        if (Mathf.Approximately(denom, 0f))
        {
            // Ray is parallel to the plane
            return false;
        }

        float t = Vector3.Dot(planePoint - rayOrigin, planeNormal) / denom;
        if (t < 0f)
        {
            // Intersection point is behind the ray origin
            return false;
        }

        hitPoint = rayOrigin + rayDir * t;
        return true;
    }
}