using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

public static class ZoneLoader
{
    public static async void GoToZone(Vector3Int nextZone, Direction dir)
    {
        // Fadeout screen

        // Save the state
        await StateUpdater.UpdateFiles();

        // Delete all the stuff from the screen
        // TODO - Add all entities under a children or something and just recursively kill all entities
        // Also just do a quick clear tiles in the tilemaps

        // Set the new zone to go
        var newZoneDelta = GameState.GetInstance()._PlayerState.currentZone - nextZone;
        GameState.GetInstance()._PlayerState.currentZone = nextZone;

        // Load / create the next zone
        bool isFirstTime;
        ZoneState newZone = GetZone(nextZone, out isFirstTime);
        
        // Set the zone

        // Set the player's position
        if (isFirstTime)
        {
            MovePlayerToSpawnPos();
        }
        else
        {
            MovePlayer(Util.ToVector3(newZone.PortalInfo.Find(x => x.directionToGo == dir).entityInfo.pos));
        }

        // Load the zone to screen / this is done inside the ZoneState constructor
    }

    private static void MovePlayerToSpawnPos()
    {
        System.Random _r = new System.Random((int) DateTimeOffset.Now.ToUnixTimeMilliseconds());
        ZoneState zoneState = GameState.GetInstance()._ZoneState;

        bool validSpawn = false;
        Vector3 spawnPos = new Vector3();
        while (!validSpawn)
        {
            validSpawn = true;

            spawnPos = new Vector3(
                _r.Next(zoneState.Height/4, zoneState.Height*3/4),
                _r.Next(zoneState.Width/4, zoneState.Width*3/4),
                0
            );

            // print("Possible spawn pos: " + spawnPos);

            for (int i = 0; validSpawn && i < 8; i++)
            {
                Vector3Int checkPos = new Vector3Int(
                    (int) spawnPos.x + Util.SURROUNDING_X[i],
                    (int) spawnPos.y + Util.SURROUNDING_Y[i],
                    0
                );
                validSpawn &= !SceneReferences.boundsTilemap.HasTile(checkPos);
            }
        }

        MovePlayer(spawnPos);
    }

    private static void MovePlayer(Vector3 pos)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = pos;
        GameState.GetInstance()._PlayerState.position = pos;
    }

    private static ZoneState GetZone(Vector3Int zone, out bool isFirstTime)
    {
        PlayerState playerState = GameState.GetInstance()._PlayerState;
        UniverseData universeData = GameState.GetInstance()._UniverseData;

        string zoneFolder = $"{PlayerPrefs.GetString("univPath")}/dim{playerState.currentZone.z}/zones/{playerState.currentZone.x},{playerState.currentZone.y}";

        // If there's no file with the current zone, create it and store it
        // If there is, read it and load it
        string zoneDataPath = $"{zoneFolder}/{playerState.currentZone.x}.{playerState.currentZone.y}.zone";

        SZoneState newZoneState;
        isFirstTime = false;

        Debug.Log("Zone: " + zoneDataPath);
        if (!File.Exists(zoneDataPath))
        {
            Debug.Log("Creating new zone...");
            // Generate all the surrounding ones?
            Directory.CreateDirectory(zoneFolder);
            
            // Create one and write it
            newZoneState = TerrainGenerator.GenerateNewZone_v0(universeData.seed1, playerState.currentZone);
            string zoneDat = JsonConvert.SerializeObject(newZoneState);
            
            FileStream fs = File.Create(zoneDataPath);
            fs.Close();

            File.WriteAllText(zoneDataPath, zoneDat, Encoding.UTF8);

            if (playerState.currentZone == Vector3Int.zero) isFirstTime = true;
        }
        else
        {
            string zoneDat = File.ReadAllText(zoneDataPath);
            newZoneState = JsonConvert.DeserializeObject<SZoneState>(zoneDat);
            // print(zoneDat);
            // print(newZoneState.tileIDs.Count);
        }

        return new ZoneState(newZoneState);
    }

    public static void LoadZone()
    {

    }

    public static void ResetScene()
    {
        // Delete the tilemaps
        // Delete the entities
        // Delete the items
        // Delete EVERYTHING HAHAHAHHAHA
    }
}