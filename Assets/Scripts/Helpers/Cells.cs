

using UnityEngine;

public static class Cells
{
    public static Vector2Int[] Cardinal;

    private static readonly Vector2Int[] CardinalAndDiagonal;

    static Cells()
    {
        Cardinal = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        CardinalAndDiagonal = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
        };
    }
}