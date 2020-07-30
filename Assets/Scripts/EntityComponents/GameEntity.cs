using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    protected int id;

    public int ID { get => id; set => id = value; }

    public static GameObject GenerateGameEntity(int entityID, Vector3 gameEntityPos)
    {
        GameEntityData gameItemData = AssetLoader.GetEntityData(entityID);

        if (gameItemData != null)
        {
            var g = GameObject.Instantiate(gameItemData.gameObject, gameEntityPos, Quaternion.identity);
            return g;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static GameObject GenerateGameItem(int itemID, Vector3 pos, ItemStack stack)
    {
        GameItemData gameItemData = AssetLoader.GetItemData(itemID);

        if (gameItemData != null)
        {
            var g = GameObject.Instantiate(AssetLoader.itemPrefab, pos, Quaternion.identity);
            
            g.GetComponent<SpriteRenderer>().sprite = AssetLoader.GetItemData(itemID).sprite;

            var cEntity = g.GetComponent<CollectibleEntity>();
            cEntity.SetCollectible(stack);
            return g;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}