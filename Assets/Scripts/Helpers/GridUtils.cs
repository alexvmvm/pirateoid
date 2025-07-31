using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tests whether two segments AB and CD intersect.
/// Returns true if they cross (including touching endpoints),
/// and outputs the intersection point when there is a single one.
/// Colinear‑overlap returns false but can be detected via 'colinear' out flag.
/// </summary>
public static class GridUtils
{
    public static bool InBounds(this Vector2Int cell)
    {
        var map = Find.Map;

        return cell.x >= 0 && cell.y >= 0 && cell.x < map.width && cell.y < map.height;
    }
}