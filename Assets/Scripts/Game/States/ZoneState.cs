using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using System;

public class ZoneState
{
    /*
        IDEAS:
        Create a encoded class in which there's all of the data needed to load this one
        tiles as an int
        map entities as an ID array as well?
        map items as a list of something with v3 and ID
    */

    private Dictionary<SVector2Int, int> tileIDs;
    private GameObject[][] mapEntities;
    private List<CollectableEntity> mapItems;
    private int height;
    private int width;
    private SVector2Int currentZone;

    public GameObject[][] MapEntities { get => mapEntities; set => mapEntities = value; }
    public List<CollectableEntity> MapItems { get => mapItems; set => mapItems = value; }
    public int Height { get => height; set => height = value; }
    public int Width { get => width; set => width = value; }
    public SVector2Int CurrentZone { get => currentZone; set => currentZone = value; }
    public Dictionary<SVector2Int, int> TileIDs { get => tileIDs; set => tileIDs = value; }

    public ZoneState(SZoneState zs)
    {
        this.Height = zs.height;
        this.Width = zs.width;
        this.CurrentZone = zs.zoneIndex;

        MapEntities = new GameObject[width][];
        // Tiles = new int[width][];

        for (int i = 0; i < width; i++)
        {
            MapEntities[i] = new GameObject[height];
            // Tiles[i] = new int[height];
        }

        MapItems = new List<CollectableEntity>();
        TileIDs = new Dictionary<SVector2Int, int>();

        LoadToScene(zs);
    }

    public void AddEntity(Vector2Int pos, GameObject e)
    {
        // TODO - CHECK
        // Overrides the current block
        MapEntities[pos.x][pos.y] = e;
    }

    public void DestroyEntity(Vector2Int pos)
    {
        var g = MapEntities[pos.x][pos.y];
        if (g != null)
        {
            var be = g.GetComponent<BreakableEntity>();
            be.Break();
            MapEntities[pos.x][pos.y] = null;
        }
    }

    public void AddItem(CollectableEntity i)
    {
        MapItems.Add(i);
    }

    public void ChangeTile(int newTileID, Vector2Int newTilePos)
    {
        // TODO - If there's nothing above it (entities, structures, etc), check that
        TileIDs[newTilePos] = newTileID;
    }

    public void LoadToScene(SZoneState zs)
    {
        // Setting the tilemap
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                Debug.Log($"({x},{y})");
                var tileInfo = AssetLoader.GetTileData(zs.tileIDs[new Vector2Int(x, y)]);

                if (tileInfo.collidable)
                {
                    SceneReferences.boundsTilemap.SetTile(new Vector3Int(x, y, 0), tileInfo.tile);
                }
                else
                {
                    SceneReferences.baseTilemap.SetTile(new Vector3Int(x, y, 0), tileInfo.tile);
                }
            }
        }

        // Setting the entities TODO ples
        foreach (var e in zs.entityInfo)
        {
            GameObject eG = GameEntity.GenerateGameEntity(e.entityID, new Vector3(e.pos.x, e.pos.y, e.pos.z));
            switch (e.entityType)
            {
                case EntityType.BREAKABLE:
                    eG.AddComponent<BreakableEntity>();
                    this.AddEntity(new Vector2Int(e.pos.x, e.pos.y), eG);

                    break;

                case EntityType.STATIC:
                    throw new NotImplementedException();

                default:
                    throw new Exception();
            }
            eG.transform.position = new Vector3(e.pos.x, e.pos.y, e.pos.z);
        }

        // Setting the collectibles
        foreach (var c in zs.collectibleInfo)
        {
            GameObject collectible = GameEntity.GenerateGameEntity(c.id, c.pos);
            var ce = collectible.AddComponent<CollectableEntity>();
            ce.SetCollectible(c.itemStack);
        }
    }
}