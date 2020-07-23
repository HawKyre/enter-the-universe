using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;

public class PrototypeTerrainGenerator : TerrainGenerator
{
    // Static fields
    private static double LAKE_MAX_RADIUS = 12;
    private static double LAKE_MIN_RADIUS = 4;
    private static float RIVER_THICKNESS = 450f;
    private static int BEZIER_ACCURACY = 95;
    private static int RIVER_BEZIER_WIDTH = 4;
    private static int BORDER_THICKNESS = 3;
    private static int FOREST_MIN_RADIUS = 6;
    private static int FOREST_MAX_RADIUS = 15;
    private static Vector3 treeCenterer = new Vector3(0.5f, 0.3f, 0);

    // NEEDS TO BE GONE 🦀
    public static ZoneState GenerateZone(long seed, Vector2Int zoneIndex)
    {
        int h = 64;
        int w = 64;
        ZoneState zs = new ZoneState(100, 100, zoneIndex);

        // Generate base map
        for (int y = -BORDER_THICKNESS; y < h + BORDER_THICKNESS; y++)
        {
            for (int x = -BORDER_THICKNESS; x < w + BORDER_THICKNESS; x++)
            {
                if (y < 0 || x < 0 || y >= h || x >= w)
                {
                    // Bounds tile
                    // boundsTilemap.SetTile(new Vector3Int(x, y, 0), AssetLoader.GetTileID("bounds_tile").tile);
                    zs.Tiles[x][y] = AssetLoader.GetTileID("bounds_tile");
                }
                else
                {
                    // Ground tile
                    // baseTilemap.SetTile(new Vector3Int(x, y, 0), AssetLoader.GetTileID("ground_tile").tile);
                    zs.Tiles[x][y] = AssetLoader.GetTileID("ground_tile");
                }
            }
        }

        System.Random r = new System.Random((int) seed);

        // Generate a river
        GenerateBezierRiver(r, zs, h, w);

        // Generate a lake
        GenerateLake(r, zs, h, w);

        // Generate trees
        GenerateTrees(r, zs, h, w);

        // TODO Maybe we need this
        // baseTilemap.RefreshAllTiles();

        return zs;
    }

    private static void GenerateLake(System.Random lr, ZoneState zs, int h, int w)
    {

        // Select a random center point
        int lake_centerX = lr.Next(w);
        int lake_centerY = lr.Next(h);
        Vector2 lake_center = new Vector2(lake_centerX, lake_centerY);

        // Select an x, y radius pair
        double lake_radiusX = LAKE_MIN_RADIUS + lr.NextDouble() * (LAKE_MAX_RADIUS - LAKE_MIN_RADIUS);
        double lake_radiusY = LAKE_MIN_RADIUS + lr.NextDouble() * (LAKE_MAX_RADIUS - LAKE_MIN_RADIUS);

        // Select those cells which are in the ellipse
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                float dist = InsideEllipse(lake_center, pos, lake_radiusX, lake_radiusY);
                
                if (dist <= 1)
                {
                    if (lr.NextDouble() > dist * dist)
                    {
                        zs.Tiles[x][y] = AssetLoader.GetTileID("bounds_tile");
                    }
                }
            }
        }
    }


    private static void GenerateRiver(System.Random lr, ZoneState zs, int h, int w)
    {   

        int river_side = lr.Next();
        

        // Map the positions to the tilemap
        Vector3Int river_startPos = new Vector3Int();
        Vector3Int river_endPos = new Vector3Int();


        if (river_side % 2 == 0)
        {
            // up to down
            river_startPos.y = 0;
            river_endPos.y = h;

            river_startPos.x = lr.Next() % w;
            river_endPos.x = lr.Next() % w;
        }
        else
        {
            // right to left
            river_startPos.x = 0;
            river_endPos.x = w;

            river_startPos.y = lr.Next() % h;
            river_endPos.y = lr.Next() % h;
        }
        
        var river_direction = river_endPos - river_startPos;
        var river_normal = new Vector3Int(river_direction.y, -river_direction.x, 0);

        // Generate a line between startPos and endPos
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                float a1 = Mathf.Abs(Vector3.Dot(pos - river_startPos, river_normal));
                
                // TODO - IMPROVE FOR FUCKS SAKE
                if (a1 < RIVER_THICKNESS)
                {
                    if (a1 / RIVER_THICKNESS < 0.7f || lr.NextDouble() > a1 / RIVER_THICKNESS)
                    {
                        zs.Tiles[x][y] = AssetLoader.GetTileID("bounds_tile");
                    }
                }
            }
        }
    }

    private static void GenerateBezierRiver(System.Random lr, ZoneState zs, int h, int w)
    {
        int river_side = lr.Next();
        

        // Map the positions to the tilemap
        Vector3Int river_startPos = new Vector3Int();
        Vector3Int river_endPos = new Vector3Int();


        if (river_side % 2 == 0)
        {
            // up to down
            river_startPos.y = 0;
            river_endPos.y = h;

            river_startPos.x = lr.Next() % w;
            river_endPos.x = lr.Next() % w;
        }
        else
        {
            // right to left
            river_startPos.x = 0;
            river_endPos.x = w;

            river_startPos.y = lr.Next() % h;
            river_endPos.y = lr.Next() % h;
        }

        // Get a random point around the middle
        List<Vector2> controlPoints = new List<Vector2>();

        controlPoints.Add(Util.ToVector2(river_startPos));
        controlPoints.Add(new Vector2(lr.Next(0, w/4), lr.Next(h/4, 3*h/4)));
        controlPoints.Add(Util.ToVector2(river_endPos));

        BezierCurve bezier = new BezierCurve(controlPoints);

        for (int i = 0; i < BEZIER_ACCURACY; i++)
        {
            double t = i / (BEZIER_ACCURACY + 0.0);
            var point = bezier.GetPoint(t);
            Vector3Int tilePos = Util.ToVector3Int(point);

            zs.Tiles[tilePos.x][tilePos.y] = AssetLoader.GetTileID("bounds_tile");

            // TODO - change
            for (int x = -RIVER_BEZIER_WIDTH / 2; x < (RIVER_BEZIER_WIDTH + 1) / 2; x++)
            {
                // boundsTilemap.SetTile(tilePos + new Vector3Int(x, 0, 0), AssetLoader.GetTileID("bounds_tile").tile);
                // baseTilemap.SetTile(tilePos + new Vector3Int(x, 0, 0), null);

                zs.Tiles[tilePos.x + x][tilePos.y] = AssetLoader.GetTileID("bounds_tile");
            }
        }

        /* BEZIER CURVE
        Define a bezier curve
        For every point, iterate at most 8? times using binary search to find the closest point in the curve 
        Calculate the distance to P1 to P2
        if it's less than threshold, delete tile

        for tile in map:
            t = 0.5
            for i(1,8) times:
                d1 = distance of tile.pos to bezier(t + 1/2.0^i)
                d2 = distance of tile.pos to bezier(t - 1/2.0^i)
                if d1 > d2, t += 1/2.0^i
                else t -= 1/2.0^i

            if distance of tile.pos bezier(t) < RIVER_THICKNESS
                add tile to bounds
        */
    }

    private static void GenerateTrees(System.Random lr, ZoneState zs, int h, int w)
    {
        // Select 2 to 5 forest starting points
        // Populate them with a elliptical circle
        // Add random trees
        // Make sure to fix the z index later

        int forest_count = lr.Next(2, 5);
        bool[][] treeGrid = new bool[h][];
        for (int i = 0; i < w; i++)
        {
            treeGrid[i] = new bool[w];
        }

        for (int i = 0; i < forest_count; i++)
        {
            Vector3Int forestCenter = new Vector3Int();
            forestCenter.x = lr.Next(w * 1/6, w * 5/6);
            forestCenter.y = lr.Next(h * 1/6, h * 5/6);

            // int forestRadius = lr.Next(FOREST_MIN_RADIUS, FOREST_MAX_RADIUS);

            int forest_xSpan = lr.Next(FOREST_MIN_RADIUS, FOREST_MAX_RADIUS);
            int forest_ySpan = lr.Next(FOREST_MIN_RADIUS, FOREST_MAX_RADIUS);

            int maxManhattan = (forest_xSpan + forest_ySpan) / 2;

            // Debug.Log($"{forestCenter} -- {forestRadius}");

            for (int x = forestCenter.x - forest_xSpan / 2; x <= forestCenter.x + forest_xSpan / 2; x++)
            {
                for (int y = forestCenter.y - forest_ySpan / 2; y <= forestCenter.y + forest_ySpan / 2; y++)
                {
                    var vPos = new Vector3Int(x, y, 0);

                    // TODO - check
                    if (treeGrid[y][x] || CloseToBounds(vPos, zs))
                        continue;

                    Vector3 tilePos = vPos + treeCenterer;
                    float manhattanDistance = Math.Abs(tilePos.x - forestCenter.x) + Math.Abs(tilePos.y - forestCenter.y);
                    float relativeMHD = manhattanDistance / maxManhattan;

                    if (lr.NextDouble() > relativeMHD * 2)
                    {
                        treeGrid[y][x] = true;
                        
                        GameObject treeEntity = GameEntity.GenerateGameEntity(1, tilePos);
                        treeEntity.AddComponent<BreakableEntity>();

                        // treeEntity.EntityObject.transform.position = tilePos;

                        GameState.GetInstance()._ZoneState.AddEntity(new Vector2Int(x, y), treeEntity);
                    }
                }
            }
        }
    }

    private static float InsideEllipse(Vector2 center, Vector3Int point, double rX, double rY)
    {
        return Mathf.Pow((point.x - center.x), 2) / Mathf.Pow((float)rX, 2) + Mathf.Pow((point.y - center.y), 2) / Mathf.Pow((float)rY, 2);
    }

    private static float InsideCircle(Vector2 center, Vector3Int point, float r)
    {
        return (Mathf.Pow((point.x - center.x), 2) + Mathf.Pow((point.y - center.y), 2)) / r*r;
    }
    
    private static bool CloseToBounds(Vector3Int pos, ZoneState zs)
    {
        for (int i = 0; i < 8; i++)
        {
            if (SceneReferences.boundsTilemap.HasTile(pos + new Vector3Int(Util.SURROUNDING_X[i], Util.SURROUNDING_Y[i], 0)))
                return true;
        }
        return false;
    }
}