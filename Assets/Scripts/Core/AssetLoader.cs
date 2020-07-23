using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetLoader
{
    public static bool dataLoaded = false;

    // public static string dataPath = "./data/entitydata.json";
    // public static string texturesPath = "Assets/Textures";
    public static string addrDataPath = "Assets/data";
    public static string addrTexturePath = "Assets/data/textures";
    public static string addrTilePath = "Assets/data/tiles";

    private static Dictionary<int, GameEntityData> entityInfoDict;
    private static Dictionary<int, GameTileData> tileInfoDict;

    private static Dictionary<string, int> entityTagTranslator;
    private static Dictionary<string, int> tileTagTranslator;

    public static void LoadGameData()
    {
        // TODO - still it can probably be improved
        LoadEntityData();
        LoadTileData();


        dataLoaded = true;
    }

    public static GameEntityData GetEntityData(string entityTag)
    {
        return entityInfoDict[entityTagTranslator[entityTag]];
    }

    public static GameTileData GetTileData(string tileTag)
    {
        return tileInfoDict[tileTagTranslator[tileTag]];
    }

    public static GameTileData GetTileData(int v)
    {
        return tileInfoDict[v];
    }

    public static int GetTileID(string v)
    {
        return tileTagTranslator[v];
    }

    private static async void LoadTileData()
    {
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/tiledata.json").Task;
        var tiles = JsonConvert.DeserializeObject<JsonTileData[]>(gameData.text);

        tileInfoDict = new Dictionary<int, GameTileData>();
        tileTagTranslator = new Dictionary<string, int>();

        foreach (var tile in tiles)
        {
            GameTileData tD = new GameTileData();
            tD.collidable = tile.collidable;
            tD.tile = await Addressables.LoadAssetAsync<RuleTile>($"{addrTilePath}/{Util.SnakeToCamel(tD.name)}.asset").Task;

            tileInfoDict.Add(tile.id, tD);
            tileTagTranslator.Add(tile.name, tile.id);
        }
    }

    private static async void LoadEntityData()
    {
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/entity.json").Task;
        var entities = JsonConvert.DeserializeObject<JsonEntityData[]>(gameData.text);

        entityInfoDict = new Dictionary<int, GameEntityData>();
        entityTagTranslator = new Dictionary<string, int>();

        foreach (var entity in entities)
        {
            GameEntityData eD = new GameEntityData();

            eD.name = entity.name;
            eD.type = (GameItemType) Enum.Parse(typeof(GameItemType), entity.type);
            if (eD.type == GameItemType.BREAKABLE)
            {
                eD.dropData = DropData.GetDropData(entity.drop);
                eD.minPower = entity.min_power;
                eD.hp = entity.hp;
            }

            Sprite s = await Addressables.LoadAssetAsync<Sprite>($"{addrTexturePath}/{Util.SnakeToCamel(eD.name)}.png").Task;

            eD.sprite = s;

            entityInfoDict.Add(entity.id, eD);
            entityTagTranslator.Add(entity.name, entity.id);
        }
    }


    public static GameEntityData GetData(int id)
    {
        GameEntityData data;
        entityInfoDict.TryGetValue(id, out data);

        Debug.Log($"Retrieving data for ID {id}: {data.ToString()}");

        if (data != null)
        {
            return entityInfoDict[id];
        }

        return null;
    }
}

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
    public List<ItemDropData> dropDataList;

    public DropData(List<ItemDropData> dropData)
    {
        dropDataList = dropData;
    }

    public static DropData GetDropData(string dropString)
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

            Debug.Log($"Adding drop with id {id} for string {dropString}");

            string[] iAttributes = iData[1].Split('&');
            foreach (var attr in iAttributes)
            {
                if (attr[0] == '%')
                {
                    // probability attribute
                    dData.probability = float.Parse(attr.Replace('%', ' '));
                    Debug.Log(dData.probability);
                }
                else if (attr[0] == 'c')
                {
                    // range attribute

                    if (!attr.Contains(","))
                    {
                        dData.hasRange = false;
                        Debug.Log("WERRR"  +attr);
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

        return new DropData(dropData);
    }
}

public class JsonEntityData
{
    public int id;
    public string name;
    public string type;
    public string drop;
    public int hp;
    public int min_power;
}

public class GameEntityData
{
    public Sprite sprite;
    public string name;
    public GameItemType type;
    public DropData dropData;
    public int hp;
    public int minPower;

    public override string ToString()
    {
        return $"{name} / {type.ToString()}";
    }
}

public class JsonTileData
{
    public int id;
    public string name;
    public bool collidable;
}

public class GameTileData
{
    public RuleTile tile;
    public string name;
    public bool collidable;
}

public enum GameItemType
{
    TILE,
    ITEM,
    BREAKABLE,
    LIVING,
    STRUCTURE
}