using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class ZoneLoader
{
    public static string zoneDataPath;

    public static async void GoToZone(Vector3Int nextZone, Direction dir)
    {
        List<Task> resetOps = new List<Task>();

        // Fadeout screen


        // Save the state
        resetOps.Add(ResetScene());
        resetOps.Add(StateUpdater.UpdateFiles());

        // Delete all the stuff from the screen
        // TODO - Add all entities under a children or something and just recursively kill all entities
        // Also just do a quick clear tiles in the tilemaps
        // resetOps.Add(ResetScene());

        await Task.WhenAll(resetOps);

        Debug.Log("Done unloading previous zone.");

        // Set the new zone to go
        var newZoneDelta = GameState.GetInstance()._PlayerState.currentZone - nextZone;
        GameState.GetInstance()._PlayerState.currentZone = nextZone;

        // Load the next zone
        ZoneState newZone = GetZone(nextZone);

        GameState.GetInstance()._ZoneState = newZone;

        Direction mirroredDir = (Direction) (((int) dir + 2) % 4);
        MovePlayer(Util.ToVector3(newZone.PortalInfo.Find(x => x.directionToGo == mirroredDir).entityInfo.pos));

        Debug.Log("Loaded next zone");

        // Load the zone to screen / this is done inside the ZoneState constructor

        // GG!
        Debug.Log("Current zone: " + nextZone);
    }

    private static void GenerateSpawnPos()
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

    private static ZoneState GetZone(Vector3Int zone)
    {
        PlayerState playerState = GameState.GetInstance()._PlayerState;
        UniverseData universeData = GameState.GetInstance()._UniverseData;

        string zoneFolder = $"{PlayerPrefs.GetString("univPath")}/dim{playerState.currentZone.z}/zones/{playerState.currentZone.x},{playerState.currentZone.y}";

        // If there's no file with the current zone, create it and store it
        // If there is, read it and load it
        zoneDataPath = $"{zoneFolder}/{playerState.currentZone.x}.{playerState.currentZone.y}.zone";

        SZoneState newZoneState;

        Debug.Log("Zone: " + zoneDataPath);
        if (!File.Exists(zoneDataPath))
        {
            Debug.Log("Creating new zone...");
            // Generate all the surrounding ones?
            Directory.CreateDirectory(zoneFolder);
            
            // Create one and write it
            newZoneState = TerrainGenerator.GenerateNewZone_v0(universeData.seed1, playerState.currentZone);
            string zoneDat = JsonConvert.SerializeObject(newZoneState);

            Debug.Log("New portals: " + newZoneState.portalInfo);
            
            FileStream fs = File.Create(zoneDataPath);
            fs.Close();

            File.WriteAllText(zoneDataPath, zoneDat, Encoding.UTF8);
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

    public static void LoadFirstZone()
    {
        GameState.GetInstance()._ZoneState = GetZone(Vector3Int.zero);
        SetupCameraConfiner(GameState.GetInstance()._ZoneState.Height, GameState.GetInstance()._ZoneState.Width);
        GenerateSpawnPos();
    }

    public static async Task ResetScene()
    {        
        Debug.Log("Deleting tiles!");

        // Delete the tilemaps
        SceneReferences.boundsTilemap.ClearAllTiles();
        SceneReferences.baseTilemap.ClearAllTiles();

        Debug.Log("Deleted tiles.");

        // Delete the entities - I think I can't do this in a task
        for (int i = 0; i < SceneReferences.entityParent.childCount; i++)
        {
            GameObject.Destroy(SceneReferences.entityParent.GetChild(i).gameObject);
        }

        Debug.Log("Deleted entities");

        // Delete the items
        for (int i = 0; i < SceneReferences.entityParent.childCount; i++)
        {
            GameObject.Destroy(SceneReferences.entityParent.GetChild(i).gameObject);
        }
        // Delete EVERYTHING HAHAHAHHAHA

        Debug.Log("Scene reset!");
    }

    private static void SetupCameraConfiner(int h, int w)
    {
        Vector3 bottomLeftCorner = Vector3.zero;
        var points = new Vector2[4];
        points[0] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y - 1);
        points[1] = new Vector2(bottomLeftCorner.x + w + 1, bottomLeftCorner.y - 1);
        points[2] = new Vector2(bottomLeftCorner.x + w + 1, bottomLeftCorner.y + h + 1);
        points[3] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y + h + 1);
        SceneReferences.cameraConfiner.points = points;
        SceneReferences.cameraConfiner.SetPath(0, new List<Vector2>(points));
    }
}