using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class RectUtils
{
    public static RectInt ContractBy(this RectInt rect, int width, int height) 
        => new(rect.x + width, rect.y + height, rect.width - width * 2, rect.height - height * 2);
    public static Rect ContractBy(this Rect rect, float amount)
        => ContractBy(rect, amount, amount); 
    public static Rect ContractBy(this Rect rect, float width, float height) 
        => new Rect(rect.x + width, rect.y + height, rect.width - width * 2, rect.height - height * 2);	
    
    public static RectInt ExpandToContain(this RectInt rect1, RectInt rect2)
    {
        // Calculate the min and max bounds of the expanded rectangle
        int xMin = Mathf.Min(rect1.xMin, rect2.xMin);
        int yMin = Mathf.Min(rect1.yMin, rect2.yMin);
        int xMax = Mathf.Max(rect1.xMax, rect2.xMax);
        int yMax = Mathf.Max(rect1.yMax, rect2.yMax);

        // Create and return the new expanded RectInt
        return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    public static RectInt ExpandToContain(this RectInt rect1, Vector2Int cell)
    {
        return ExpandToContain(rect1, new RectInt(cell.x, cell.y, 1, 1));
    }

    public static bool RectsTouchOrIntersect(RectInt rect1, RectInt rect2)
    {
        // Check if rectangles intersect or touch
        bool intersects = rect1.xMax >= rect2.x && rect1.x <= rect2.xMax &&
                          rect1.yMax >= rect2.y && rect1.y <= rect2.yMax;

        return intersects;
    }

    public static IEnumerable<Vector2Int> Cells(this RectInt rect)
    {
        for(var x = rect.xMin; x < rect.xMax; x++)
        {
            for(var y = rect.yMin; y < rect.yMax; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }
    
    public static Rect ToWorldRect(this RectInt rect, Tilemap map)
    {
        var t1 = map.CellToWorld(rect.min.ToVector3Int());
        var t2 = map.CellToWorld(rect.max.ToVector3Int());

        return new Rect(t1.x, t1.y, t2.x - t1.x, t2.y - t1.y);
    }

    public static IEnumerable<Vector2Int> EdgeCells(this RectInt rect)
    {
        int x = rect.xMin;
        int y = rect.yMin;

        var xMax = rect.xMax;
        var yMax = rect.yMax;

        var xMin = rect.xMin;
        var yMin = rect.yMin;
        
        for(; x < xMax; x++)
        {
            yield return new Vector2Int(x, y);
        }
        x--;
        y++;
        for(; y < yMax; y++ )
        {
            yield return new Vector2Int(x, y);
        }
        y--;
        x--;
        for(; x >= xMin; x-- )
        {
            yield return new Vector2Int(x, y);
        }
        x++;
        y--;
        for(; y >= yMin; y-- )
        {
            yield return new Vector2Int(x, y);
        }
    }

    public static RectInt ExpandBy(this RectInt rect, int amount)
    {
        return new RectInt(
            rect.xMin - amount,   // Move left by amount
            rect.yMin - amount,   // Move down by amount
            rect.width + (amount * 2), // Expand width by twice the amount (left + right)
            rect.height + (amount * 2) // Expand height by twice the amount (top + bottom)
        );
    }

    public static Rect ExpandBy(this Rect rect, float amount)
    {
        return new Rect(
            rect.xMin - amount,   // Move left by amount
            rect.yMin - amount,   // Move down by amount
            rect.width + (amount * 2), // Expand width by twice the amount (left + right)
            rect.height + (amount * 2) // Expand height by twice the amount (top + bottom)
        );
    }

    public static bool IsCorner(this RectInt rect, Vector2Int point)
    {
        if( rect.xMin == point.x && rect.yMax - 1 == point.y)
            return true;
        else if( rect.xMax - 1 == point.x && rect.yMax - 1 == point.y)
            return true;
        else if( rect.xMin == point.x && rect.yMin == point.y)
            return true;
        else if( rect.xMax - 1 == point.x && rect.yMin == point.y)
            return true;
        else
            return false;
    }

    public static Rect WorldToGuiRect(this Rect rect)
    {
        var min = rect.min.WorldToGuiPoint();
        var max = rect.max.WorldToGuiPoint();
        var width = Mathf.Abs(max.x - min.x);
        var height = Mathf.Abs(max.y - min.y);

        return new Rect(min.x, min.y - height, width, height);
    }

    public static Rect ScreenToGUIRect(this Rect screenRect)
    {
        // Invert the Y position to account for GUI coordinates being flipped vertically
        float guiY = Screen.height - screenRect.y - screenRect.height;

        // Return the new Rect in GUI space
        return new Rect(screenRect.x, guiY, screenRect.width, screenRect.height);
    }

    public static Rect GetScreenCoordinates(this RectTransform uiElement, ref Vector3[] worldCorners)
    {
        uiElement.GetWorldCorners(worldCorners);
        var result = new Rect(
                        worldCorners[0].x,
                        worldCorners[0].y,
                        worldCorners[2].x - worldCorners[0].x,
                        worldCorners[2].y - worldCorners[0].y);
        
        return result;
    }

    public static void SplitVerticallyAmount(this Rect rect, out Rect top, out Rect bottom, float topHeight = 0f)
    {
        top = new Rect(rect.x, rect.y, rect.width, topHeight);
        bottom =  new Rect(rect.x, rect.y + topHeight , rect.width, rect.height - topHeight);
    }

    public static void SplitHorizontallyAmount(this Rect rect, out Rect left, out Rect right, float leftWidth = 0f)
    {
        left = new Rect(rect.x, rect.y, leftWidth, rect.height);
        right = new Rect(rect.x + leftWidth, rect.y, rect.width - leftWidth, rect.height);
    }

    public static void SplitVerticallyAmount(this RectInt rect, out RectInt top, out RectInt bottom, int topHeight = 0)
    {
        top = new RectInt(rect.x, rect.y, rect.width, topHeight);
        bottom =  new RectInt(rect.x, rect.y + topHeight , rect.width, rect.height - topHeight);
    }

    public static void SplitHorizontallyAmount(this RectInt rect, out RectInt left, out RectInt right, int leftWidth = 0)
    {
        left = new RectInt(rect.x, rect.y, leftWidth, rect.height);
        right = new RectInt(rect.x + leftWidth, rect.y, rect.width - leftWidth, rect.height);
    }

    public static void SplitHorizontallyPercent(this Rect rect, out Rect left, out Rect right, float split = 0.5f)
    {
        left = new Rect(rect.x, rect.y, rect.width * split, rect.height);
        right = new Rect(rect.x + rect.width * split, rect.y, rect.width * (1f - split), rect.height);
    }

    public static void SplitVerticallyPercent(this Rect rect, out Rect top, out Rect bottom, float split = 0.5f)
    {
        top = new Rect(rect.x, rect.y, rect.width, rect.height * split);
        bottom =  new Rect(rect.x, rect.y + rect.height * split, rect.width, (1f - split) * rect.height);
    }

    public static void SplitHorizontallyPercent(this RectInt rect, out RectInt left, out RectInt right, float split = 0.5f)
    {
        left = new RectInt(rect.x, rect.y, Mathf.FloorToInt(rect.width * split), rect.height);
        right = new RectInt(rect.x + Mathf.FloorToInt(rect.width * split), rect.y, Mathf.CeilToInt(rect.width * (1f - split)), rect.height);
    }

    private static readonly Vector3[] worldCorners = new Vector3[4];
    public static Rect GUIRect(this RectTransform rectTransform)
    {
        // Get the screen rect of the RectTransform
        rectTransform.GetWorldCorners(worldCorners);

        // Convert the corners to screen coordinates
        var screenBottomLeft = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]);
        var screenTopRight = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]);

        var inverseScale = 1f / RootUI.Scale;

        return new Rect(
            screenBottomLeft.x * inverseScale, 
            Screen.height * inverseScale - screenTopRight.y * inverseScale,
            screenTopRight.x * inverseScale - screenBottomLeft.x * inverseScale,
            screenTopRight.y * inverseScale - screenBottomLeft.y * inverseScale);
    }

    public static Rect ToRect(this Bounds bounds)
    {
        // Use the center and size of the bounds to create a Rect.
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        // Rect requires bottom-left corner and size.
        return new Rect(
            center.x - size.x / 2, // Bottom-left corner X
            center.y - size.y / 2, // Bottom-left corner Y
            size.x,                // Width
            size.y                 // Height
        );
    }

    public static Rect ToRect(this RectInt rect)
    {
        return new Rect(rect.position, rect.size);
    }

    public static bool IsZero(this RectInt rect) 
        => rect.size == Vector2Int.zero;

    /// <summary>
    /// Returns the closest point inside the RectInt to the given point.
    /// If the point is already inside the rect, it returns the point itself.
    /// </summary>
    public static Vector2Int ClosestPointInRect(this RectInt rect, Vector2Int point)
    {
        int clampedX = Mathf.Clamp(point.x, rect.xMin, rect.xMax - 1);
        int clampedY = Mathf.Clamp(point.y, rect.yMin, rect.yMax - 1);
        return new Vector2Int(clampedX, clampedY);
    }
}
