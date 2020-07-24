using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    protected int id;

    public int ID { get => id; set => id = value; }

    public static GameObject GenerateGameEntity(int entityID, Vector3 gameEntityPos)
    {
        GameEntityData gameItemData = AssetLoader.GetData(entityID);

        if (gameItemData != null)
        {
            var g = GameObject.Instantiate(GameState.GetInstance()._DefaultEntity, gameEntityPos, Quaternion.identity);
            var ge = g.AddComponent<GameEntity>();
            ge.ID = entityID;
            
            g.name = "Entity ID [" + entityID + "]";
            g.GetComponent<SpriteRenderer>().sprite = gameItemData.sprite;

            print("POS: : : " + gameEntityPos);
            // g.transform.position = gameEntityPos;

            print(g.transform.position);

            return g;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}