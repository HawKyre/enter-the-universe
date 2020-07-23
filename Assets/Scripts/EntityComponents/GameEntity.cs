using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    protected int id;

    public int ID { get => id; set => id = value; }

    public static GameObject GenerateGameEntity(int entityID, Vector3 gameEntityPos)
    {
        // EntityInfo eInfo;
        // GameState.GetInstance()._EntityDictionary.eDict.TryGetValue(entityID, out eInfo);

        GameEntityData gameItemData = AssetLoader.GetData(entityID);

        if (gameItemData != null)
        {
            var g = GameObject.Instantiate(GameState.GetInstance()._DefaultEntity);
            var ge = g.AddComponent<GameEntity>();
            ge.ID = entityID;
            
            g.name = "Entity ID [" + entityID + "]";
            g.GetComponent<SpriteRenderer>().sprite = gameItemData.sprite;

            g.transform.position = gameEntityPos;

            return g;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}