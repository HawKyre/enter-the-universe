using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneReferences : MonoBehaviour
{
    public static Tilemap baseTilemap;
    public static Tilemap boundsTilemap;
    public static Transform entityParent;
    public static Transform itemParent;
    public static PolygonCollider2D cameraConfiner;

    private void Awake()
    {
        baseTilemap = GameObject.FindGameObjectWithTag("baseTilemap").GetComponent<Tilemap>();
        boundsTilemap = GameObject.FindGameObjectWithTag("boundsTilemap").GetComponent<Tilemap>();
        entityParent = GameObject.FindGameObjectWithTag("entityParent").transform;
        itemParent = GameObject.FindGameObjectWithTag("itemParent").transform;
        cameraConfiner = GameObject.FindGameObjectWithTag("cameraConfiner").GetComponent<PolygonCollider2D>();
    }
}
