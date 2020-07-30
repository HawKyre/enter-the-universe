using System;
using System.Text.RegularExpressions;
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

    public static Vector3 ToVector3(Vector3Int v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static string SnakeToCamel(string name)
    {
        MatchEvaluator me = new MatchEvaluator((m) => UpperCaseMatch(m));
        return Regex.Replace(name, @"_([a-z])", me);
    }

    private static string UpperCaseMatch(Match m)
    {
        return (char) (m.Value[1] - 32) + "";
    }

    public static int ZZtoZ(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        
        if (x == 0 && y == 0) return 0;
        
        int n = Math.Max(Math.Abs(x), Math.Abs(y));
        
        int angle = (int) (Math.Atan2(-y, x) * 360/(2*Math.PI));
        if (angle < 0) angle = angle + 360;

            
        if (angle < 45 || angle >= 225)
        {
            // negative value
            return -ZZtoZ(new Vector2Int(-x, -y));
        }
        
        // positive value
        if (Math.Abs(y) == n)
        {
            return d1(n) + n - x;
        }
        return d2(n) + n - y;
    }

    private static int d1(int n)
    {
        if (n == 0) return 0;
        n--;
        return 2*n*(n+1) + 1;
    }
    
    private static int d2(int n)
    {
        if (n == 0) return 0;
        n--;
        return 2*(n+1) + 2*n*(n+1) + 1;
    }
}