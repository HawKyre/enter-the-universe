using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

public class ZoneState
{
    private int[][] tiles;
    private GameObject[][] mapEntities;
    private List<CollectableEntity> mapItems;
    private int height;
    private int width;
    private SVector2Int currentZone;

    public int[][] Tiles { get => tiles; set => tiles = value; }
    public GameObject[][] MapEntities { get => mapEntities; set => mapEntities = value; }
    public List<CollectableEntity> MapItems { get => mapItems; set => mapItems = value; }
    public int Height { get => height; set => height = value; }
    public int Width { get => width; set => width = value; }
    public SVector2Int CurrentZone { get => currentZone; set => currentZone = value; }

    public ZoneState(int width, int height, SVector2Int currentZone)
    {
        this.Height = height;
        this.Width = width;
        this.CurrentZone = currentZone;


        MapEntities = new GameObject[width][];
        Tiles = new int[width][];

        for (int i = 0; i < width; i++)
        {
            MapEntities[i] = new GameObject[height];
            Tiles[i] = new int[height];
        }

        MapItems = new List<CollectableEntity>();
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
        Tiles[newTilePos.x][newTilePos.y] = newTileID;
    }

    public void LoadToScene()
    {
        // Setting the tilemap
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tileInfo = AssetLoader.GetTileData(tiles[x][y]);

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

        // Setting the entities
    }
}