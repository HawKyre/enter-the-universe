using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public Inventory playerInventory;
    public SVector3 position;

    public SVector2Int currentZone;

    public PlayerState(Inventory playerInventory, SVector3 position)
    {
        this.playerInventory = playerInventory;
        this.position = position;
        currentZone = Vector2Int.zero;
    }
}