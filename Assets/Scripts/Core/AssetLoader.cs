using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Linq;
using System.Threading.Tasks;

public static class AssetLoader
{
    public static bool dataLoaded = false;

    public static string addrDataPath = "Assets/data";
    public static string addrEntityPath = "Assets/data/entityModels";
    public static string addrItemPath = "Assets/data/textures";
    public static string addrTilePath = "Assets/data/tiles";
    public static string addrPrefabPath = "Assets/data/prefabs";

    private static Dictionary<int, GameItemData> itemInfoDict;
    private static Dictionary<int, GameEntityData> entityInfoDict;
    private static Dictionary<int, GameTileData> tileInfoDict;

    private static Dictionary<string, int> itemTagTranslator;
    private static Dictionary<string, int> entityTagTranslator;
    private static Dictionary<string, int> tileTagTranslator;

    public static GameObject itemPrefab;

    public static async void LoadGameData()
    {
        await Addressables.InitializeAsync().Task;

        List<Task> loadOps = new List<Task>();

        // TODO - I'm quite happy with this ngl
        loadOps.Add(LoadEntityData());
        loadOps.Add(LoadItemData());
        loadOps.Add(LoadTileData());
        loadOps.Add(LoadItemPrefab());

        await Task.WhenAll(loadOps);
        dataLoaded = true;

        Debug.Log("LOADED");
    }

    private static async Task LoadItemPrefab()
    {
        itemPrefab = await Addressables.LoadAssetAsync<GameObject>($"{addrPrefabPath}/itemPrefab.prefab").Task;
    }

    private static async Task LoadItemData()
    {
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/itemdata.txt").Task;
        List<GameItemData> entityData = DataDecoder.DecodeItemData(gameData.text);

        itemInfoDict = entityData.ToDictionary(x => x.ID);
        itemTagTranslator = entityData.ToDictionary(x => x.name, x => x.ID);

        foreach (var item in itemInfoDict)
        {
            item.Value.sprite = await Addressables.LoadAssetAsync<Sprite>($"{addrItemPath}/{item.Value.name}.png").Task;
            if (item.Value.sprite != null)
            {
                Debug.Log("I could load " + item.Value.name);
            }
        }
    }

    private static async Task LoadTileData()
    {
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/tiledata.txt").Task;
        List<GameTileData> tilesData = DataDecoder.DecodeTileData(gameData.text);

        tileInfoDict = tilesData.ToDictionary(x => x.ID);
        tileTagTranslator = tilesData.ToDictionary(x => x.name, x => x.ID);

        foreach (var tileData in tileInfoDict)
        {
            tileData.Value.tile = await Addressables.LoadAssetAsync<RuleTile>($"{addrTilePath}/{tileData.Value.name}.asset").Task;
        }
    }

    private static async Task LoadEntityData()
    {
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/entitydata.txt").Task;
        List<GameEntityData> entityData = DataDecoder.DecodeEntityData(gameData.text);

        entityInfoDict = entityData.ToDictionary(x => x.ID);
        entityTagTranslator = entityData.ToDictionary(x => x.name, x => x.ID);

        foreach (var entity in entityInfoDict)
        {
            entity.Value.gameObject = await Addressables.LoadAssetAsync<GameObject>($"{addrEntityPath}/{Util.SnakeToCamel(entity.Value.name)}.prefab").Task;
        }
    }

    
    public static GameEntityData GetEntityData(string entityTag)
    {
        if (!entityTagTranslator.ContainsKey(entityTag))
        {
            throw new NullReferenceException("Attemped to index by " + entityTag);
        }
        return entityInfoDict[entityTagTranslator[entityTag]];
    }

    public static GameEntityData GetEntityData(int entityID)
    {
        if (!entityInfoDict.ContainsKey(entityID))
        {
            throw new NullReferenceException("Attemped to index by " + entityID);
        }
        return entityInfoDict[entityID];
    }

    public static int GetEntityID(string entityTag)
    {
        if (!entityTagTranslator.ContainsKey(entityTag))
        {
            throw new NullReferenceException("Tried to access string " + entityTag + " but no value was found");
        }
        return entityTagTranslator[entityTag];
    }

    public static GameTileData GetTileData(string tileTag)
    {
        if (!tileTagTranslator.ContainsKey(tileTag))
        {
            throw new NullReferenceException("Attemped to index by " + tileTag);
        }
        return tileInfoDict[tileTagTranslator[tileTag]];
    }

    public static GameTileData GetTileData(int tileID)
    {
        if (!tileInfoDict.ContainsKey(tileID))
        {
            throw new NullReferenceException("Attemped to index by " + tileID);
        }
        return tileInfoDict[tileID];
    }

    public static int GetTileID(string v)
    {
        if (!tileTagTranslator.ContainsKey(v))
        {
            throw new NullReferenceException("Tried to access string " + v + " but no value was found");
        }
        return tileTagTranslator[v];
    }

    public static GameItemData GetItemData(string itemTag)
    {
        if (!itemTagTranslator.ContainsKey(itemTag))
        {
            throw new NullReferenceException("Attemped to index by " + itemTag);
        }
        return itemInfoDict[itemTagTranslator[itemTag]];
    }

    public static GameItemData GetItemData(int itemID)
    {
        if (!itemInfoDict.ContainsKey(itemID))
        {
            throw new NullReferenceException("Attemped to index by " + itemID);
        }
        return itemInfoDict[itemID];
    }

    public static int GetItemID(string v)
    {
        if (!itemTagTranslator.ContainsKey(v))
        {
            throw new NullReferenceException("Tried to access string " + v + " but no value was found");
        }
        return itemTagTranslator[v];
    }

}

[System.Serializable]
public class ItemDropData
{
    public int id;
    public int fixedDropCount;
    public bool hasRange;
    public Range dropRange;
    public float probability;

    private int activeDropCount = -1;

    public int GetDropCount()
    {
        if (hasRange)
        {
            activeDropCount = UnityEngine.Random.Range(0f, 1f) < probability ? dropRange.GetInRange() : 0;
        }
        else
        {
            activeDropCount = UnityEngine.Random.Range(0f, 1f) < probability ? fixedDropCount : 0;
        }
        Debug.Log(hasRange + "-" + probability + "-" + activeDropCount + "-" + fixedDropCount);
        return activeDropCount;
    }

    public ItemStack GetItemStack()
    {
        if (activeDropCount < 0)
        {
            GetDropCount();
        }

        return new ItemStack(id, activeDropCount);
    }
}

public class DropData
{
    public static List<ItemDropData> GetDropData(string dropString)
    {
        List<ItemDropData> dropData = new List<ItemDropData>();

        string[] itemsData = dropString.Split(';');
        foreach (var i in itemsData)
        {
            ItemDropData dData = new ItemDropData();
            string[] iData = i.Split(':');
            int id = int.Parse(iData[0]);

            dData.id = id;
            dData.probability = 1;

            string[] iAttributes = iData[1].Split('&');
            foreach (var attr in iAttributes)
            {
                if (attr[0] == '%')
                {
                    // probability attribute
                    dData.probability = float.Parse(attr.Replace('%', ' '));
                }
                else if (attr[0] == 'c')
                {
                    // range attribute

                    if (!attr.Contains(","))
                    {
                        dData.hasRange = false;
                        dData.fixedDropCount = int.Parse(attr.Replace('c', ' '));

                    }
                    else
                    {
                        string[] dCount = attr.Replace('c', ' ').Split(',');
                        dData.hasRange = true;
                        dData.dropRange = new Range(int.Parse(dCount[0]), int.Parse(dCount[1]));
                    }
                }
            }

            dropData.Add(dData);
        }

        return dropData;
    }

    
}

public class GameItemData
{
    public int ID;
    public string name;
    public Sprite sprite;
}

public class GameEntityData
{
    public int ID;
    public string name;
    public GameObject gameObject;

    public override string ToString()
    {
        return $"{name} / Entity";
    }
}

public class GameTileData
{
    public int ID;
    public string name;
    public RuleTile tile;
    public bool collidable;
}