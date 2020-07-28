
// Asynchronously writes the file periodically to keep the latest progress saved
using System;
using System.IO;
using Newtonsoft.Json;

public class StateUpdater
{
    public static void UpdateFiles()
    {
        // Write the univ.dat file
        WriteUnivData();

        // Write the player.dat file
        WritePlayerData();

        // Write the current zone's file
        WriteZoneData();
    }

    private static async void WriteZoneData()
    {
        SZoneState currentZoneState = GameState.GetInstance()._ZoneState.Serialize();
        string zoneDat = JsonConvert.SerializeObject(currentZoneState);
        // Write async
    }

    private static async void WritePlayerData()
    {
        string playerDat = JsonConvert.SerializeObject(GameState.GetInstance()._PlayerState);
        // Write async
    }

    private static async void WriteUnivData()
    {
        
    }
}