using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SZoneState
{
    public Dictionary<SVector2Int, int> tileIDs;

    public List<GameEntityInfo> entityInfo;
    public List<CollectibleInfo> collectibleInfo;
    public List<PortalInfo> portalInfo;
    public int height;
    public int width;
    public Vector3Int zoneIndex;

    public SZoneState(int h, int w, Vector3Int zoneIndex)
    {
        this.height = h;
        this.width = w;
        this.zoneIndex = zoneIndex;

        entityInfo = new List<GameEntityInfo>();
        collectibleInfo = new List<CollectibleInfo>();
        portalInfo = new List<PortalInfo>();

        tileIDs = new Dictionary<SVector2Int, int>();
    }
}

public class GameEntityInfo
{
    public int entityID;
    public SVector3Int pos;
    public EntityType entityType;
}

public class CollectibleInfo
{
    public ItemStack itemStack;
    public SVector3 pos;

    public CollectibleInfo(ItemStack item, Vector3 pos)
    {
        itemStack = item;
        this.pos = pos;
    }
}