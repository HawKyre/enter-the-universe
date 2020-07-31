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

    private static bool[][] isCollidable;
    private static bool[][] entityGrid;

    public static SZoneState GenerateZone(long seed, Vector3Int zoneIndex)
    {
        int h = 128;
        int w = 128;
        SZoneState zs = new SZoneState(h, w, zoneIndex);
        isCollidable = new bool[w][];
        entityGrid = new bool[w][];

        for (int i = 0; i < w; i++)
        {
            isCollidable[i] = new bool[h];
            entityGrid[i] = new bool[h];
        }

        // Generate base map
        for (int y = -BORDER_THICKNESS; y < h + BORDER_THICKNESS; y++)
        {
            for (int x = -BORDER_THICKNESS; x < w + BORDER_THICKNESS; x++)
            {
                if (y < 0 || x < 0 || y >= h || x >= w)
                {
                    // Bounds tile
                    // boundsTilemap.SetTile(new Vector3Int(x, y, 0), AssetLoader.GetTileID("boundsTile").tile);
                    zs.tileIDs.Add(new SVector2Int(x, y), AssetLoader.GetTileID("boundsTile"));
                }
                else
                {
                    // Ground tile
                    // baseTilemap.SetTile(new Vector3Int(x, y, 0), AssetLoader.GetTileID("groundTile").tile);
                    zs.tileIDs.Add(new SVector2Int(x, y), AssetLoader.GetTileID("groundTile"));
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

        // Generate portals
        GenerateRightPortal(r, zs, h, w);
        GenerateLeftPortal(r, zs, h, w);
        GenerateTopPortal(r, zs, h, w);
        GenerateBottomPortal(r, zs, h, w);

        Debug.Log(zs.tileIDs.Count);

        return zs;
    }

    private static void GenerateRightPortal(System.Random lr, SZoneState zs, int h, int w)
    {
        int x = lr.Next(w/2, w/2 + w/4);
        int y = lr.Next(h/2 - (x - w/2), h/2 + (x - w/2));
        x += w/4;
        Vector3Int vPos = new Vector3Int(x, y, 0);

        while (entityGrid[y][x] || CloseToBounds(vPos, zs))
        {
            x = lr.Next(w/2, w/2 + w/4);
            y = lr.Next(h/2 - (x - w/2), h/2 + (x - w/2));
            x += w/4;
            vPos = new Vector3Int(x, y, 0);
        }

        // Debug.Log("Added portal entity from right at " + vPos);

        AddPortalEntity(zs, vPos, zs.zoneIndex + Vector3Int.right, Direction.RIGHT);
    }

    private static void GenerateLeftPortal(System.Random lr, SZoneState zs, int h, int w)
    {
        int x = lr.Next(w/2 - w/4, w/2);
        int y = lr.Next(h/2 - (w/2 - x), h/2 + (w/2 - x));
        x -= w/4;
        Vector3Int vPos = new Vector3Int(x, y, 0);

        while (entityGrid[y][x] || CloseToBounds(vPos, zs))
        {
            x = lr.Next(w/2 - w/4, w/2);
            y = lr.Next(h/2 - (w/2 - x), h/2 + (w/2 - x));
            x -= w/4;
            vPos = new Vector3Int(x, y, 0);
        }

        // Debug.Log("Added portal entity from left at " + vPos);

        AddPortalEntity(zs, vPos, zs.zoneIndex + Vector3Int.left, Direction.LEFT);
    }


    private static void GenerateTopPortal(System.Random lr, SZoneState zs, int h, int w)
    {
        int y = lr.Next(h/2, h/2 + h/4);
        int x = lr.Next(w/2 - (y - h/2), w/2 + (y - h/2));
        y += h/4;
        Vector3Int vPos = new Vector3Int(x, y, 0);

        while (entityGrid[y][x] || CloseToBounds(vPos, zs))
        {
            y = lr.Next(h/2, h/2 + h/4);
            x = lr.Next(w/2 - (y - h/2), w/2 + (y - h/2));
            y += h/4;
            vPos = new Vector3Int(x, y, 0);
        }

        // Debug.Log("Added portal entity from top at " + vPos);

        AddPortalEntity(zs, vPos, zs.zoneIndex + Vector3Int.up, Direction.TOP);
    }

    private static void GenerateBottomPortal(System.Random lr, SZoneState zs, int h, int w)
    {
        int y = lr.Next(h/2 - h/4, h/2);
        int x = lr.Next(w/2 - (h/2 - y), w/2 + (h/2 - y));
        y -= h/4;
        Vector3Int vPos = new Vector3Int(x, y, 0);

        while (entityGrid[y][x] || CloseToBounds(vPos, zs))
        {
            y = lr.Next(h/2 - h/4, h/2);
            x = lr.Next(w/2 - (h/2 - y), w/2 + (h/2 - y));
            y -= h/4;
            vPos = new Vector3Int(x, y, 0);
        }

        // Debug.Log("Added portal entity from bottom at " + vPos);

        AddPortalEntity(zs, vPos, zs.zoneIndex + Vector3Int.down, Direction.BOTTOM);
    }

    private static void AddPortalEntity(SZoneState zs, Vector3Int vPos, Vector3Int nextZone, Direction direction)
    {
        PortalInfo p = new PortalInfo();

        GameEntityInfo e = new GameEntityInfo();
        e.entityID = 2;
        e.entityType = EntityType.INTERACTABLE | EntityType.HOVERABLE;
        e.pos = vPos;


        p.entityInfo = e;
        p.nextZone = nextZone;
        p.directionToGo = direction;

        zs.portalInfo.Add(p);
    }

    private static void GenerateLake(System.Random lr, SZoneState zs, int h, int w)
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
                        zs.tileIDs[new SVector2Int(x, y)] = AssetLoader.GetTileID("boundsTile");
                        isCollidable[x][y] = true;
                    }
                }
            }
        }
    }


    private static void GenerateRiver(System.Random lr, SZoneState zs, int h, int w)
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
                        zs.tileIDs[new SVector2Int(x, y)] = AssetLoader.GetTileID("boundsTile");
                        isCollidable[x][y] = true;
                    }
                }
            }
        }
    }

    private static void GenerateBezierRiver(System.Random lr, SZoneState zs, int h, int w)
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

            zs.tileIDs[new SVector2Int(tilePos.x, tilePos.y)] = AssetLoader.GetTileID("boundsTile");
            isCollidable[tilePos.x][tilePos.y] = true;

            // TODO - change
            for (int x = -RIVER_BEZIER_WIDTH / 2; x < (RIVER_BEZIER_WIDTH + 1) / 2; x++)
            {
                // boundsTilemap.SetTile(tilePos + new Vector3Int(x, 0, 0), AssetLoader.GetTileID("boundsTile").tile);
                // baseTilemap.SetTile(tilePos + new Vector3Int(x, 0, 0), null);

                zs.tileIDs[new SVector2Int(tilePos.x + x, tilePos.y)] = AssetLoader.GetTileID("boundsTile");
                try
                {
                    isCollidable[tilePos.x + x][tilePos.y] = true;
                }
                catch (Exception)
                {
                    // Welp can't do much
                }
            }
        }
    }

    private static void GenerateTrees(System.Random lr, SZoneState zs, int h, int w)
    {
        // Select 2 to 5 forest starting points
        // Populate them with a elliptical circle
        // Add random trees
        // Make sure to fix the z index later

        int forest_count = lr.Next(2, 5);

        for (int i = 0; i < forest_count; i++)
        {
            Vector3Int forestCenter = new Vector3Int();
            forestCenter.x = lr.Next(w * 1/6, w * 5/6);
            forestCenter.y = lr.Next(h * 1/6, h * 5/6);

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
                    if (entityGrid[x][y] || CloseToBounds(vPos, zs))
                        continue;

                    float manhattanDistance = Math.Abs(vPos.x - forestCenter.x) + Math.Abs(vPos.y - forestCenter.y);
                    float relativeMHD = manhattanDistance / maxManhattan;

                    if (lr.NextDouble() > relativeMHD * 2)
                    {
                        entityGrid[x][y] = true;

                        GameEntityInfo e = new GameEntityInfo();
                        e.entityID = AssetLoader.GetEntityID("prototypeTree");
                        e.pos = new SVector3Int(x, y, 0);
                        e.entityType = EntityType.BREAKABLE;
                        
                        zs.entityInfo.Add(e);
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
    
    private static bool CloseToBounds(Vector3Int pos, SZoneState zs)
    {
        try
        {
            for (int i = 0; i < 8; i++)
            {
                int x = pos.x + Util.SURROUNDING_X[i];
                int y = pos.y + Util.SURROUNDING_Y[i];
                if (isCollidable[x][y])
                    return true;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            return true;
        }
        return false;
    }
}