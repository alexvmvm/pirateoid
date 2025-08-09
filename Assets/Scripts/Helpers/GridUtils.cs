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
    public static bool InBounds(this Vector2Int cell, Map map)
    {
        return cell.x >= 0 && cell.y >= 0 && cell.x < map.width && cell.y < map.height;
    }

    public static bool AdjacentTo(this Vector2Int position, Vector2Int other)
    {
        for(var x = position.x - 1; x <= position.x + 1; x++)
        {
            for(var y = position.y - 1; y <= position.y + 1; y++)
            {
                if( x == position.x && y == position.y )   
                    continue;

                if( x == other.x && y == other.y )
                    return true;
            }
        }

        return false;
    }

    public static float PathCost(this Vector2Int cell)
    {        
        return 1f;
    }
}