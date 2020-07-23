using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class GameState : MonoBehaviour
{
    // Singleton instance
    private static GameState Instance;
    
    // TODO - Load it from assets instead of dragging because dragging bad
    [SerializeField] private GameObject defaultEntity;
    [SerializeField] 

    private Transform breakableEntityParent;
    private Transform staticEntityParent;

    // TODO - Probably should find the reference in the start of somewhere else, it makes no sense to have this here
    private new ParticleSystem particleSystem;
    private UIIngameState currentUIState;

    private UniverseData universeData;
    private PlayerState playerState;
    private ZoneState zoneState;

    public GameObject _DefaultEntity { get => defaultEntity; private set => defaultEntity = value; }
    public ParticleSystem _ParticleSystem { get => particleSystem; private set => particleSystem = value; }
    public UIIngameState _CurrentUIState { get => currentUIState; set => currentUIState = value; }

    public PlayerState _PlayerState { get => playerState; set => playerState = value; }
    public ZoneState _ZoneState { get => zoneState; set => zoneState = value; }
    public UniverseData _UniverseData { get => universeData; set => universeData = value; }

    private void Awake()
    {
        // We do this so that we keep the state across reloads I guess
        // TODO - move to wherever it is that we transition from zone to zone instead of here, since it might give problems when going to the menu from game and we want to keep it only when we switch scene to the same one to get to the next zone
        DontDestroyOnLoad(this.gameObject);

        // This should be moved somewhere else please for the love of gos it makes no sense to have this here when it should be in the main scene
        // maube with the if it's not needed but still think about it
        if (!AssetLoader.dataLoaded) AssetLoader.LoadGameData();

        // This goes in awake since we only need this once every gaming moment
        LoadUniverseData();

        Instance = this.gameObject.GetComponent<GameState>();

        _ParticleSystem = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
    }

    private void LoadUniverseData()
    {
        string univDataPath = PlayerPrefs.GetString("univData") + "/univ.dat";
        string univDat = File.ReadAllText(univDataPath);
        this.universeData = JsonConvert.DeserializeObject<UniverseData>(univDat);
    }

    private void Start()
    {
        if (playerState == null) 
        {
            // read the player state
            string playerDataPath = PlayerPrefs.GetString("univData") + "/player.dat";
            string playerDat = File.ReadAllText(playerDataPath);
            this.playerState = JsonConvert.DeserializeObject<PlayerState>(playerDat);
        }

        string zoneFolder = $"{PlayerPrefs.GetString("univData")}/zones/{playerState.currentZone.x},{playerState.currentZone.y}";
        // If there's no file with the current zone, create it and store it
        // If there is, read it and load it
        string zoneDatPath = $"{zoneFolder}/{playerState.currentZone.x}.{playerState.currentZone.y}.zone";

        if (!Directory.Exists(zoneFolder))
        {
            // Generate all the surrounding ones?
            Directory.CreateDirectory(zoneFolder);
            
            // Create one and write it
            ZoneState newZoneState = TerrainGenerator.GenerateNewZone_v0(universeData.seed1, playerState.currentZone);
            string zoneDat = JsonConvert.SerializeObject(newZoneState);
            File.WriteAllText(zoneDatPath, zoneDat);
        }
        else
        {
            string zoneDat = File.ReadAllText(zoneDatPath);
            this.zoneState = JsonConvert.DeserializeObject<ZoneState>(zoneDat);
        }
    }

    public static GameState GetInstance()
    {
        return Instance;
    }
}
