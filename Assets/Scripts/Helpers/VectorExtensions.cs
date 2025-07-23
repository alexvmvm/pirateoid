using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorUtils 
{
    public static Vector2Int ToGroundCell(this Vector3 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    public static Vector2Int ToGroundCellRounded(this Vector3 v) => new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    public static Vector2Int ToGroundCell(this Vector2 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    public static Vector2 ScreenToGroundPosition(this Vector2 v) => Camera.main.ScreenToWorldPoint(v);
    public static Vector2 ScreenToGroundPosition(this Vector3 v) => Camera.main.ScreenToWorldPoint(v);
    public static Vector2Int ScreenToGroundCell(this Vector3 v) => ScreenToGroundCell(new Vector2(v.x, v.y));
    public static Vector2Int ScreenToGroundCell(this Vector2 v) => v.ScreenToGroundPosition().ToGroundCell();
    public static Vector2 Center(this Vector2Int v) => new(v.x + 0.5f, v.y + 0.5f);
    public static Vector2 Center(this Vector3 v) => new(Mathf.FloorToInt(v.x) + 0.5f, Mathf.FloorToInt(v.y) + 0.5f);
    public static Vector2 TopLeft(this Vector2Int v) => new(v.x, v.y + 1f);
    public static Vector3 ToVector3(this Vector2 v) => new(v.x, v.y);
    public static Vector3 ToVector3(this Vector2Int v) => new(v.x, v.y);
    public static Vector3Int ToVector3Int(this Vector2Int v) => new(v.x, v.y);
    public static Vector2 ToVector2(this Vector3 v) => new(v.x, v.y);
    public static Vector2 ToVector2(this Vector2Int v) => new(v.x, v.y);
    public static Vector2Int ToVector2IntFloor(this Vector2 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    public static Vector2Int ToVector2IntCeil(this Vector2 v) => new(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
    public static Vector2 Centroid(this IEnumerable<Vector2> points)
    {
        if( points.NullOrEmpty() )
            return Vector2.zero; // Return a zero vector if there are no points

        var sumX = 0f;
        var sumY = 0f;
        
        foreach (var point in points)
        {
            sumX += point.x;
            sumY += point.y;
        }

        var count = points.Count();

        // Divide the sum of x and y coordinates by the number of points to get the centroid
        return new Vector2(sumX / count, sumY / count);
    }

    public static Vector2 WorldToGuiPoint(this Vector3 position)
    {
        var p = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(position));
        p.y = UIScreen.Height - p.y;
        return p;
    }

    public static Vector2 ScreenToGUIPoint(this Vector3 screenPoint)
    {
        // Flip the y-coordinate and adjust for GUI's top-left origin
        return new Vector2(screenPoint.x, Screen.height - screenPoint.y);
    }
    
    public static Vector2 WorldToGuiPoint(this Vector2 position) => WorldToGuiPoint((Vector3)position);

    public static Vector2 GUIToScreenPosition(this Vector2 guiPosition)
    {
        // Flip the Y coordinate to convert from top-left origin (IMGUI) to bottom-left (screen space)
        float screenY = Screen.height - guiPosition.y;

        // Return the new screen position with the adjusted Y coordinate
        return new Vector2(guiPosition.x, screenY);
    }
}
