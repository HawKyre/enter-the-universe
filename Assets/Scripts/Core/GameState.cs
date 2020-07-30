using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Text;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    // Singleton instance
    private static GameState Instance;
    
    // TODO - Load it from assets instead of dragging because dragging bad
    [SerializeField] private GameObject defaultEntity;

    private string zoneDataPath;

    private Transform breakableEntityParent;
    private Transform staticEntityParent;

    private Transform playerTransform;

    private PolygonCollider2D cameraConfiner;

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
        // DontDestroyOnLoad(this.gameObject);

        // This should be moved somewhere else please for the love of gos it makes no sense to have this here when it should be in the main scene
        // maybe with the if it's not needed but still think about it
        if (!AssetLoader.dataLoaded) AssetLoader.LoadGameData();

        // This goes in awake since we only need this once every gaming moment
        LoadUniverseData();

        Instance = this.gameObject.GetComponent<GameState>();

        _ParticleSystem = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
    }

    private void LoadUniverseData()
    {
        string univDataPath = PlayerPrefs.GetString("univPath") + "/univ.dat";
        string univDat = File.ReadAllText(univDataPath);
        this.universeData = JsonConvert.DeserializeObject<UniverseData>(univDat);
    }

    public string GetZonePath()
    {
        return zoneDataPath;
    }

    // TODO - FUCKING GARBAGE
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnZoneLoaded;
    }

    private void OnZoneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindReferences();
        
        // if (playerState == null)
        // {
        //     // read the player state
        //     string playerDataPath = PlayerPrefs.GetString("univPath") + "/player.dat";
        //     string playerDat = File.ReadAllText(playerDataPath);
        //     this.playerState = JsonConvert.DeserializeObject<PlayerState>(playerDat);
        // }

        

        

        // Setup the camera confiner
        Vector3 bottomLeftCorner = Vector3.zero;
        var points = new Vector2[4];
        points[0] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y - 1);
        points[1] = new Vector2(bottomLeftCorner.x + _ZoneState.Width + 1, bottomLeftCorner.y - 1);
        points[2] = new Vector2(bottomLeftCorner.x + _ZoneState.Width + 1, bottomLeftCorner.y + _ZoneState.Height + 1);
        points[3] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y + _ZoneState.Height + 1);
        cameraConfiner.points = points;
        cameraConfiner.SetPath(0, new List<Vector2>(points));
    }

    private void BindReferences()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraConfiner = GameObject.FindGameObjectWithTag("CameraConfiner").GetComponent<PolygonCollider2D>();
        Debug.Log("References binded: " + playerTransform + " / " + cameraConfiner);
    }

    private void Update()
    {
        SaveState();
        UpdatePlayerState();
    }

    private void UpdatePlayerState()
    {
        Debug.Log(_PlayerState + " - " + playerTransform);
        _PlayerState.position = playerTransform.position;
    }

    private float savingTime = 20f;
    private void SaveState()
    {
        savingTime -= Time.deltaTime;
        if (savingTime < 0)
        {
            savingTime = 20f;
            StateUpdater.UpdateFiles();
        }
    }

    public static GameState GetInstance()
    {
        return Instance;
    }

    public void SetPlayerPosition(Vector3? pos = null)
    {
        if (pos == null)
        {
            playerTransform.position = _PlayerState.position;
        }
        else
        {
            playerTransform.position = (Vector3) pos;
        }
    }
}
