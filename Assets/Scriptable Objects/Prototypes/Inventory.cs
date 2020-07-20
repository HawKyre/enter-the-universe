
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Enter The Dimension/Inventory", order = 0)]
public class Inventory : ScriptableObject
{
    public List<ItemStack> inventory;

    public void LoadInventory()
    {
        if (inventory == null)
        {
            inventory = new List<ItemStack>();
        }
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
}