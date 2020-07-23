using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    
    private void Start()
    {
        playerInventory = new Inventory();
    }

    private void Update() {
        
    }

    public void AddItemToInventory(ItemStack i)
    {
        playerInventory.AddItem(i);
    }
}
