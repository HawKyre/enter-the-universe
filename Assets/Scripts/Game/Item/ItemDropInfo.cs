using System.Collections.Generic;
using UnityEngine;

public class ItemDropInfo
{
    public int itemID;
    public Range count;

    public ItemDropInfo(int itemID, Range count)
    {
        this.itemID = itemID;
        this.count = count;
    }
}