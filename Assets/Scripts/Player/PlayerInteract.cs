using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Tilemap crossTilemap;
    [SerializeField] private Tile crossTile;

    private Vector3Int? lastPos;

    private void Start() {
        lastPos = null;
    }

    private void Update() {

        if (GameState.GetInstance()._CurrentUIState != UIIngameState.GAME)
        {
            crossTilemap.ClearAllTiles();
            return;
        }

        // Get the tile we're looking at
        // Render a selection square at it
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentPos = Vector3Int.FloorToInt(mousePos);

        if (lastPos != currentPos)
        {
            lastPos = currentPos;
            crossTilemap.ClearAllTiles();
            crossTilemap.SetTile(currentPos, crossTile);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            GameState.GetInstance()._ZoneState.DestroyEntity(new Vector2Int(currentPos.x, currentPos.y));
        }
    }
}
