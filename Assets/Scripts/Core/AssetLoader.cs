using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;

public static class AssetLoader
{
    public static string dataPath = "./data/id.json";
    public static string texturesPath = "Assets/Textures";
    public static Dictionary<int, GameItemData> gameDict;

    public static void LoadGameData()
    {
        // TODO - improve probably

        string gameData = File.ReadAllText(Application.dataPath + dataPath);
        var items = JsonConvert.DeserializeObject<JsonItemData[]>(gameData);

        Debug.Log("PLEASE " + items.Length);

        gameDict = new Dictionary<int, GameItemData>();

        foreach (var item in items)
        {
            GameItemData d = new GameItemData();
            d.name = item.name;
            // d.sprite = (Sprite) AssetDatabase.LoadAssetAtPath(texturesPath + "/" + Util.SnakeToCamel(d.name) + ".png", typeof(Sprite));

            d.sprite = (Sprite) AssetDatabase.LoadAssetAtPath($"{texturesPath}/{Util.SnakeToCamel(d.name)}.png", typeof(Sprite));

            Debug.Log(texturesPath + "/" + Util.SnakeToCamel(d.name) + ".png");

            d.type = (GameItemType) Enum.Parse(typeof(GameItemType), item.type);
            if (d.type == GameItemType.BREAKABLE)
            {
                d.dropData = DropData.GetDropData(item.drop);
                d.minPower = item.min_power;
                d.hp = item.hp;
            }
            gameDict.Add(item.id, d);
        }
    }

    public static GameItemData GetData(int id)
    {
        GameItemData data;
        gameDict.TryGetValue(id, out data);

        Debug.Log($"Retrieving data for ID {id}: {data.ToString()}");

        if (data != null)
        {
            return gameDict[id];
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
    public double probability;

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
        return activeDropCount;
    }

    public ItemStack GetItemStack()
    {
        if (activeDropCount < 0)
        {
            throw new Exception();
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

            Debug.Log($"Adding drop with id {id} for string {dropString}");

            string[] iAttributes = iData[1].Split('&');
            foreach (var attr in iAttributes)
            {
                if (attr[0] == '%')
                {
                    // probability attribute
                    dData.probability = double.Parse(attr.Replace('%', ' '));
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

public class JsonItemData
{
    public int id;
    public string name;
    public string type;
    public string drop;
    public int hp;
    public int min_power;
}

public class GameItemData
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

public enum GameItemType
{
    TILE,
    ITEM,
    BREAKABLE,
    LIVING,
    STRUCTURE
}