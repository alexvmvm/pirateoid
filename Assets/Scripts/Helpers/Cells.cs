

using UnityEngine;

public static class Cells
{
    public static Vector2Int[] Cardinal;

    static Cells()
    {
        Cardinal = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };
    }
}