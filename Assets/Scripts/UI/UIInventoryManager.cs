using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private static int GAP = 15;
    private static int UI_ITEM_SIZE = 150;
    private static int MAX_ITEMS_IN_ROW = 10;

    private List<UIItem> spawnedInventoryItems;

    public void RefreshInventory()
    {
        if (spawnedInventoryItems == null)
        {
            spawnedInventoryItems = new List<UIItem>();
        }
        else
        {
            for (int i = 0; i < spawnedInventoryItems.Count; i++)
            {
                GameObject.Destroy(spawnedInventoryItems[i].uiObject.gameObject);
            }
            spawnedInventoryItems = new List<UIItem>();
        }

        int row = 0;
        int column = 0;

        // Sort inventory by ID before showing it?
        GameState.GetInstance()._PlayerState.playerInventory.SortInventory((x, y) => x.ID - y.ID);

        foreach (ItemStack item in GameState.GetInstance()._PlayerState.playerInventory.GetItems())
        {
            print($"Found item {item.ID} x{item.Count} in inventory");
            GameObject itemInstance = GameObject.Instantiate(itemPrefab);
            itemInstance.transform.SetParent(this.transform);

            spawnedInventoryItems.Add(new UIItem(item.ID, item.Count, itemInstance));

            var rect = itemInstance.GetComponent<RectTransform>();
            Vector3 itemPos = new Vector3(row * (UI_ITEM_SIZE + GAP), column * (UI_ITEM_SIZE + GAP), 0);
            rect.anchoredPosition3D = itemPos;
            rect.localScale = Vector3.one;

            Image itemImage = itemInstance.transform.GetChild(0).GetComponent<Image>();
            itemImage.sprite = AssetLoader.GetData(item.ID).sprite;

            TextMeshProUGUI itemText = itemInstance.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            itemText.text = "x" + item.Count;


            row++;
            if (row > MAX_ITEMS_IN_ROW)
            {
                column++;
                row = 0;
            }
        }
    }
}
