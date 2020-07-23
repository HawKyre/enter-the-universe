using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneReferences : MonoBehaviour
{
    public static Tilemap baseTilemap;
    public static Tilemap boundsTilemap;

    private void Awake()
    {
        baseTilemap = GameObject.FindGameObjectWithTag("baseTilemap").GetComponent<Tilemap>();
        boundsTilemap = GameObject.FindGameObjectWithTag("boundsTilemap").GetComponent<Tilemap>();
    }
}
