
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemStackEntity : IGameItem
{
    private int _id;
    private int _count;
    private GameObject _itemGameObject;
    private ItemObject item;

    public int ID { get => _id; set => _id = value; }
    public int Count { get => _count; set => _count = value; }
    public GameObject ItemGameObject { get => _itemGameObject; set => _itemGameObject = value; }
    public ItemObject Item { get => item; set => item = value; }

    public ItemStackEntity(int id, int count, Vector3 spawnPos)
    {
        this.ID = id;
        this.Count = count;

        ItemInfo iInfo;
        GameState.GetInstance()._ItemDictionary.iDict.TryGetValue(id, out iInfo);

        if (iInfo != null)
        {
            this.ItemGameObject = GameObject.Instantiate(GameState.GetInstance()._DefaultItem, spawnPos, Quaternion.identity);
            this.ItemGameObject.GetComponent<SpriteRenderer>().sprite = iInfo.itemSprite;

            this.Item = this.ItemGameObject.GetComponent<ItemObject>();
            this.item.Rb2d = this.ItemGameObject.GetComponent<Rigidbody2D>();

            this.Item.ItemStack = new ItemStack(id, count);
        }
    }

    public void Move(Vector3 to)
    {
        Item.Move(to);
    }
}