using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // Singleton instance
    private static GameState Instance;

    [SerializeField] private ItemDictionary itemDictionary;
    [SerializeField] private EntityDictionary entityDictionary;
    [SerializeField] private MapObject mapObject;
    [SerializeField] private Inventory playerInventory;
    
    [SerializeField] private GameObject defaultEntity;
    [SerializeField] private GameObject defaultItem;

    private Transform breakableEntityParent;
    private Transform staticEntityParent;
    private new ParticleSystem particleSystem;
    private UIState currentUIState;

    public ItemDictionary _ItemDictionary { get => itemDictionary; private set => itemDictionary = value; }
    public EntityDictionary _EntityDictionary { get => entityDictionary; private set => entityDictionary = value; }
    public GameObject _DefaultEntity { get => defaultEntity; private set => defaultEntity = value; }
    public Transform _BreakableEntityParent { get => breakableEntityParent; private set => breakableEntityParent = value; }
    public Transform _StaticEntityParent { get => staticEntityParent; private set => staticEntityParent = value; }
    public GameObject _DefaultItem { get => defaultItem; private set => defaultItem = value; }
    public MapObject _MapObject { get => mapObject; private set => mapObject = value; }
    public Inventory _PlayerInventory { get => playerInventory; private set => playerInventory = value; }
    public ParticleSystem _ParticleSystem { get => particleSystem; private set => particleSystem = value; }
    public UIState _CurrentUIState { get => currentUIState; set => currentUIState = value; }

    private void Awake()
    {
        Instance = this.gameObject.GetComponent<GameState>();
        _ItemDictionary.LoadDictionary_v0();
        _EntityDictionary.LoadDictionary_v0();

        _BreakableEntityParent = GameObject.Find("/Game Entities/Breakable").transform;
        _StaticEntityParent = GameObject.Find("/Game Entities/Static").transform;

        _ParticleSystem = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
    }

    public static GameState GetInstance()
    {
        return Instance;
    }
}
