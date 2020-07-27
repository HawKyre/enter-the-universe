
// Asynchronously writes the file periodically to keep the latest progress saved
using System;

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

    private static void WriteZoneData()
    {
        throw new NotImplementedException();
    }

    private static void WritePlayerData()
    {
        throw new NotImplementedException();
    }

    private static void WriteUnivData()
    {
        SZoneState currentZoneState = 
    }
}