using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : IEntity
{
    protected int id;
    protected GameObject entityObject;

    public int ID { get => id; set => id = value; }
    public GameObject EntityObject { get => entityObject; set => entityObject = value; }

    public GameEntity(int entityID, Vector3 gameEntityPos)
    {
        EntityInfo eInfo;
        GameState.GetInstance()._EntityDictionary.eDict.TryGetValue(entityID, out eInfo);

        if (eInfo != null)
        {
            this.ID = entityID;
            this.EntityObject = GameObject.Instantiate(GameState.GetInstance()._DefaultEntity);
            this.EntityObject.name = "Entity ID [" + entityID + "]";
            this.EntityObject.GetComponent<SpriteRenderer>().sprite = eInfo._EntitySprite;

            // Get the sprite's physics shape
            List<Vector2> physicsShapeList = new List<Vector2>();
            this.EntityObject.GetComponent<SpriteRenderer>().sprite.GetPhysicsShape(0, physicsShapeList);
            
            // Assign it to the polygon collider
            this.EntityObject.GetComponent<PolygonCollider2D>().points = physicsShapeList.ToArray();

            switch (eInfo._EntityType)
            {
                case EntityType.BREAKABLE:
                    this.EntityObject.transform.SetParent(GameState.GetInstance()._BreakableEntityParent);
                    break;
                case EntityType.STATIC:
                    this.EntityObject.transform.SetParent(GameState.GetInstance()._StaticEntityParent);
                    break;
                default:
                    throw new Exception();
            }

            this.EntityObject.transform.position = gameEntityPos;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}