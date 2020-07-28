using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using System;
using System.Linq;

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
    private List<EntityContainer> mapEntities;
    private List<CollectibleEntity> mapItems;
    private int height;
    private int width;
    private SVector2Int currentZone;

    private SZoneState serializedZoneState;


    public List<CollectibleEntity> MapItems { get => mapItems; set => mapItems = value; }
    public int Height { get => height; set => height = value; }
    public int Width { get => width; set => width = value; }
    public SVector2Int CurrentZone { get => currentZone; set => currentZone = value; }
    public Dictionary<SVector2Int, int> TileIDs { get => tileIDs; set => tileIDs = value; }
    public List<EntityContainer> MapEntities { get => mapEntities; set => mapEntities = value; }

    public ZoneState(SZoneState zs)
    {
        this.Height = zs.height;
        this.Width = zs.width;
        this.CurrentZone = zs.zoneIndex;

        MapEntities = new List<EntityContainer>();
        // Tiles = new int[width][];

        MapItems = new List<CollectibleEntity>();
        TileIDs = new Dictionary<SVector2Int, int>();

        serializedZoneState = zs;

        LoadToScene(zs);
    }

    public void AddEntity(GameObject e, Vector2Int pos, EntityType type, int id)
    {
        // TODO - CHECK
        // Overrides the current block
        // MapEntities[pos.x][pos.y] = e;
        MapEntities.Add(new EntityContainer(e, pos, type, id));
    }

    public void DestroyEntity(Vector2Int pos)
    {
        int g_ix = mapEntities.FindIndex(x => x.position == pos);
        if (g_ix != -1 && mapEntities[g_ix].type == EntityType.BREAKABLE)
        {
            var be = mapEntities[g_ix].gameObject.GetComponent<BreakableEntity>();
            be.Break();
            mapEntities.RemoveAt(g_ix);
        }
        else
        {
            throw new Exception("Can't destroy this object or there's no object at this position");
        }
    }

    public void AddItem(CollectibleEntity i)
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
            Vector3 offset = AssetLoader.GetEntityData(e.entityID).positionOffset;
            GameObject eG = GameEntity.GenerateGameEntity(e.entityID, new Vector3(e.pos.x, e.pos.y, e.pos.z) + offset);
            switch (e.entityType)
            {
                case EntityType.BREAKABLE:
                    eG.AddComponent<BreakableEntity>();
                    this.AddEntity(eG, new Vector2Int(e.pos.x, e.pos.y), EntityType.BREAKABLE, e.entityID);

                    break;

                case EntityType.STATIC:
                    throw new NotImplementedException();

                default:
                    throw new Exception();
            }
            // eG.transform.position = new Vector3(e.pos.x, e.pos.y, e.pos.z);
        }

        // Setting the collectibles
        foreach (var c in zs.collectibleInfo)
        {
            GameObject collectible = GameEntity.GenerateGameEntity(c.itemStack.ID, c.pos);
            var ce = collectible.AddComponent<CollectibleEntity>();
            ce.SetCollectible(c.itemStack);
        }
    }

    public SZoneState Serialize()
    {
        SZoneState sz = new SZoneState(height, width, currentZone);
        sz.collectibleInfo = mapItems.Select(x => x.GetInfo()).ToList();
        sz.tileIDs = this.tileIDs;
        sz.entityInfo = new List<GameObjectInfo>();
        foreach (var e in MapEntities)
        {
            sz.entityInfo.Add(e.GetInfo());
        }

        return sz;
    }
}

public struct EntityContainer
{
    public GameObject gameObject;
    public SVector2Int position;
    public EntityType type;
    public int ID;

    public EntityContainer(GameObject gameObject, SVector2Int position, EntityType type, int iD)
    {
        this.gameObject = gameObject;
        this.position = position;
        this.type = type;
        ID = iD;
    }

    public GameObjectInfo GetInfo()
    {
        GameObjectInfo i = new GameObjectInfo();
        i.entityID = this.ID;
        i.entityType = this.type;
        i.pos = new SVector3Int(this.position.x, this.position.y, 0);
        return i;
    }
}