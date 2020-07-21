using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMap
{
    public GameObject[][] mapEntities;
    public List<CollectableEntity> mapItems;

    private static ZoneMap Instance;

    public static ZoneMap GetInstance()
    {
        return Instance;
    }

    public ZoneMap(int width, int height)
    {
        mapEntities = new GameObject[width][];
        for (int i = 0; i < width; i++)
        {
            mapEntities[i] = new GameObject[height];
        }

        mapItems = new List<CollectableEntity>();
    }

    public void AddEntity(Vector2Int pos, GameObject e)
    {
        // TODO - CHECK
        // Overrides the current block
        mapEntities[pos.x][pos.y] = e;
    }

    public void DestroyEntity(Vector2Int pos)
    {
        var g = mapEntities[pos.x][pos.y];
        if (g != null)
        {
            var be = g.GetComponent<BreakableEntity>();
            be.Break();
            mapEntities[pos.x][pos.y] = null;
        }
    }

    public void AddItem(CollectableEntity i)
    {
        mapItems.Add(i);
    }
}
