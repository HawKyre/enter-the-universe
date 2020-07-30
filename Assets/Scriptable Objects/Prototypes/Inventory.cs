using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[Serializable]
public class Inventory
{
    [JsonIgnore]
    private const int MAX_INVENTORY_SIZE = 50;
    
    private List<ItemStack> inventoryList;

    public List<ItemStack> InventoryList { get => inventoryList; private set => inventoryList = value; }

    public Inventory()
    {
        inventoryList = new List<ItemStack>(MAX_INVENTORY_SIZE);
    }

    public int Count() {return InventoryList.Count;}

    public void AddItem(ItemStack i)
    {
        int itemIndex = InventoryList.FindIndex(x => x.id == i.id);
        Debug.Log(itemIndex);

        if (itemIndex != -1)
        {
            Debug.Log("Adding to item " + i.id);
            InventoryList[itemIndex].count += i.count;
        }
        else
        {
            // Debug.Log("Adding item with ID " + i.ID);
            InventoryList.Add(i);
        }
    }

    public void SortInventory(Comparison<ItemStack> p)
    {
        InventoryList.Sort(p);
    }

    public List<ItemStack> GetItems()
    {
        return InventoryList;
    }
}