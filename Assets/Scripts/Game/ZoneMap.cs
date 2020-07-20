using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMap
{
    public IEntity[][] mapEntities;
    public List<IGameItem> mapItems;

    private static ZoneMap Instance;

    public static ZoneMap GetInstance()
    {
        return Instance;
    }

    public ZoneMap(int width, int height)
    {
        mapEntities = new IEntity[width][];
        for (int i = 0; i < width; i++)
        {
            mapEntities[i] = new IEntity[height];
        }

        mapItems = new List<IGameItem>();
    }

    public void AddEntity(Vector2Int pos, IEntity e)
    {
        // TODO - CHECK
        // Overrides the current block
        mapEntities[pos.x][pos.y] = e;
    }

    public void DestroyEntity(Vector2Int pos)
    {
        if (mapEntities[pos.x][pos.y] is BreakableEntity)
        {
            BreakableEntity b = mapEntities[pos.x][pos.y] as BreakableEntity;
            b.Break();
            mapEntities[pos.x][pos.y] = null;
        }
    }

    public void AddItem(IGameItem i)
    {
        mapItems.Add(i);
    }
}
