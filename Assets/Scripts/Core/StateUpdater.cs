
// Asynchronously writes the file periodically to keep the latest progress saved
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class StateUpdater
{

    public static async Task UpdateFiles()
    {
        List<Task> writingOps = new List<Task>();

        // Write the univ.dat file
        writingOps.Add(WriteUnivData());

        // Write the player.dat file
        writingOps.Add(WritePlayerData());

        // Write the current zone's file
        writingOps.Add(WriteZoneData());

        Debug.Log("State is being saved...");

        await Task.WhenAll(writingOps);

        Debug.Log("State saved!");
    }

    private static async Task WriteZoneData()
    {
        string zoneDataPath = ZoneLoader.zoneDataPath;
        UTF8Encoding uEnc = new UTF8Encoding();

        byte[] result = await Task<byte[]>.Run(() => {
            SZoneState currentZoneState = GameState.GetInstance()._ZoneState.Serialize();
            string zoneDat = JsonConvert.SerializeObject(currentZoneState);
            return uEnc.GetBytes(zoneDat);
        });

        // Write backup async
        using (FileStream sw = File.OpenWrite(zoneDataPath + ".bak"))
        {
            await Task.Run(() => sw.WriteAsync(result, 0, result.Length));
        }

        await Task.Run(() => File.Move(zoneDataPath, zoneDataPath + ".old"));
        await Task.Run(() => File.Move(zoneDataPath + ".bak", zoneDataPath));
        await Task.Run(() => File.Delete(zoneDataPath + ".old"));
    }

    private static async Task WritePlayerData()
    {
        string playerDataPath = PlayerPrefs.GetString("univPath") + "/player.dat";
        UTF8Encoding uEnc = new UTF8Encoding();

        byte[] result = await Task<byte[]>.Run(() => {
            string playerDat = JsonConvert.SerializeObject(GameState.GetInstance()._PlayerState);
            return uEnc.GetBytes(playerDat);
        });

        // Write backup async
        using (FileStream sw = File.OpenWrite(playerDataPath + ".bak"))
        {
            await Task.Run(() => sw.WriteAsync(result, 0, result.Length));
        }

        File.Move(playerDataPath, playerDataPath + ".old");
        File.Move(playerDataPath + ".bak", playerDataPath);
        File.Delete(playerDataPath + ".old");
    }

    private static async Task WriteUnivData()
    {
        string univDataPath = PlayerPrefs.GetString("univPath") + "/univ.dat";
        UTF8Encoding uEnc = new UTF8Encoding();

        byte[] result = await Task<byte[]>.Run(() => {
            string playerDat = JsonConvert.SerializeObject(GameState.GetInstance()._UniverseData);
            return uEnc.GetBytes(playerDat);
        });

        // Write backup async
        using (FileStream sw = File.OpenWrite(univDataPath + ".bak"))
        {
            await Task.Run(() => sw.WriteAsync(result, 0, result.Length));
        }

        File.Move(univDataPath, univDataPath + ".old");
        File.Move(univDataPath + ".bak", univDataPath);
        File.Delete(univDataPath + ".old");
    }
}