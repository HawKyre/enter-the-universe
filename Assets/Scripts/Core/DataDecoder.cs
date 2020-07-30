
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataDecoder
{
    public static List<GameTileData> DecodeTileData(string t)
    {
        List<GameTileData> l = new List<GameTileData>();
        var args = t.Split('\n').Select(x => x.Split(':'));
        foreach (var arg in args)
        {
            GameTileData g = new GameTileData();
            g.ID = int.Parse(arg[0]);
            g.name = arg[1].Trim();
            g.collidable = arg[2] == "1";

            l.Add(g);
        }

        return l;
    }

    public static List<GameEntityData> DecodeEntityData(string t)
    {
        List<GameEntityData> l = new List<GameEntityData>();
        var args = t.Split('\n').Select(x => x.Split(':'));
        foreach (var arg in args)
        {
            GameEntityData g = new GameEntityData();
            g.ID = int.Parse(arg[0]);
            g.name = arg[1].Trim();

            l.Add(g);
        }

        return l;
    }

    public static List<GameItemData> DecodeItemData(string t)
    {
        List<GameItemData> l = new List<GameItemData>();
        var args = t.Split('\n').Select(x => x.Split(':'));
        foreach (var arg in args)
        {
            GameItemData g = new GameItemData();
            g.ID = int.Parse(arg[0]);
            g.name = arg[1].Trim();

            l.Add(g);
        }

        return l;
    }
}

/*
        var gameData = await Addressables.LoadAssetAsync<TextAsset>($"{addrDataPath}/tiledata.txt").Task;
        var tiles = JsonConvert.DeserializeObject<JsonTileData[]>(gameData.text);

        tileInfoDict = new Dictionary<int, GameTileData>();
        tileTagTranslator = new Dictionary<string, int>();


        foreach (var tile in tiles)
        {
            Debug.Log("TL " + tile.name + " at path " + $"{addrTilePath}/{Util.SnakeToCamel(tile.name)}.asset" );
            GameTileData tD = new GameTileData();
            tD.collidable = tile.collidable;
            tD.tile = await Addressables.LoadAssetAsync<RuleTile>($"{addrTilePath}/{Util.SnakeToCamel(tile.name)}.asset").Task;

            tileInfoDict.Add(tile.id, tD);
            Debug.Log($"ADDING TILE {tile.name}");
            tileTagTranslator.Add(tile.name, tile.id);
        }
*/