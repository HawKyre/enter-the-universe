
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemStack : IGameItem
{
    private int _id;
    private int _count;

    public int ID { get => _id; set => _id = value; }
    public int Count { get => _count; set => _count = value; }

    public ItemStack(int id, int count)
    {
        this.ID = id;
        this.Count = count;
    }
}