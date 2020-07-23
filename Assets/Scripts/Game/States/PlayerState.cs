using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public Inventory playerInventory;
    public SVector2 position;

    public SVector2Int currentZone;

    public PlayerState(Inventory playerInventory, SVector2 position)
    {
        this.playerInventory = playerInventory;
        this.position = position;
        currentZone = Vector2Int.zero;
    }
}