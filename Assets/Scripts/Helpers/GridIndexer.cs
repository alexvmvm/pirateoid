using UnityEngine;

/// <summary>
/// Converts between a linear index (0…width*height‑1) and a 2‑D cell (x,y).
/// Row‑major layout: index = y * width + x.
/// </summary>
public class GridIndexer
{
    public readonly int Width;
    public readonly int Height;

    public int CellCount => Width * Height;

    public GridIndexer(int width, int height)
    {
        if (width  <= 0) throw new System.ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new System.ArgumentOutOfRangeException(nameof(height));

        Width  = width;
        Height = height;
    }

    /// <summary>Returns the 1‑D index for a grid cell (x,y).</summary>
    public int ToIndex(Vector2Int cell)
    {
        if (!InBounds(cell))
            throw new System.ArgumentOutOfRangeException(nameof(cell), $"Cell {cell} outside grid {Width}×{Height}");

        return cell.y * Width + cell.x;           // row‑major
    }

    /// <summary>Returns the 2‑D cell (x,y) for a 1‑D index.</summary>
    public Vector2Int ToCell(int index)
    {
        if (index < 0 || index >= CellCount)
            throw new System.ArgumentOutOfRangeException(nameof(index), $"Index {index} outside 0…{CellCount - 1}");

        int y = index / Width;
        int x = index % Width;
        return new Vector2Int(x, y);
    }

    /// <summary>True if the cell lies inside the grid bounds.</summary>
    public bool InBounds(Vector2Int cell)
        => cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;
}
