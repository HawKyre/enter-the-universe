
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public int id;
    public int count;

    public ItemStack(int id, int count)
    {
        this.id = id;
        this.count = count;
    }
}