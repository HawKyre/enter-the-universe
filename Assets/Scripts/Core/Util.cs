using UnityEngine;

public static class Util
{
    /*
    Surrounding vectors for iterating around a tile
    0 1 2
    3 x 4
    5 6 7
    */
    public static int[] SURROUNDING_X = new int[]{-1, 0, 1, -1, 1, -1, 0, 1};
    public static int[] SURROUNDING_Y = new int[]{1, 1, 1, 0, 0, -1, -1, -1};

    public static Vector2 ToVector2(Vector3Int v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3Int ToVector3Int(Vector2 v)
    {
        return new Vector3Int((int) v.x, (int) v.y, 0);
    }
}