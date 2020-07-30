using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState
{
    public Inventory playerInventory;
    public SVector3 position;

    ///<summary>
    /// (x, y) design the coordinate, z designs the dimension
    ///</summary>
    public SVector3Int currentZone;

    public PlayerState(Inventory playerInventory, SVector3 position, bool testing = false)
    {
        this.playerInventory = playerInventory;
        this.position = position;
        currentZone = new Vector3Int(0, 0, testing ? -1 : 0);
    }
}