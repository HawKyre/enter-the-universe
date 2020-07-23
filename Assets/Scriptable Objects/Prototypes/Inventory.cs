
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private const int MAX_INVENTORY_SIZE = 50;
    private List<ItemStack> inventory;

    public Inventory()
    {
        inventory = new List<ItemStack>(MAX_INVENTORY_SIZE);
    }

    public void AddItem(ItemStack i)
    {
        Debug.Log(inventory.Count + " - " + i);

        int itemIndex = inventory.FindIndex(x => x.ID == i.ID);
        Debug.Log(itemIndex);

        if (itemIndex != -1)
        {
            Debug.Log("Adding to item " + i.ID);
            inventory[itemIndex].Count += i.Count;
        }
        else
        {
            // Debug.Log("Adding item with ID " + i.ID);
            inventory.Add(i);
        }
    }

    public void SortInventory(Comparison<ItemStack> p)
    {
        inventory.Sort(p);
    }

    public List<ItemStack> GetItems()
    {
        return inventory;
    }
}