using System.Collections.Generic;
using UnityEngine;

public class BreakableEntity : MonoBehaviour
{
    private int damageStatus;
    private GameItemData bInfo;
    private int localID;

    public int DamageStatus { get => damageStatus; protected set => damageStatus = value; }

    private void Awake()
    {
        localID = this.gameObject.GetComponent<GameEntity>().ID;

        // Get the sprite's physics shape
        List<Vector2> physicsShapeList = new List<Vector2>();
        this.gameObject.GetComponent<SpriteRenderer>().sprite.GetPhysicsShape(0, physicsShapeList);
        
        // Assign it to the polygon collider
        var pc2d = this.gameObject.AddComponent<PolygonCollider2D>();
        pc2d.points = physicsShapeList.ToArray();

        bInfo = AssetLoader.GetData(localID);
    }

    // public BreakableEntity(int entityID, Vector3 gameEntityPos)
    // {
    //     // We can assume that 
    //     EntityInfo eInfo;
    //     GameState.GetInstance()._EntityDictionary.eDict.TryGetValue(entityID, out eInfo);
        
    //     if (eInfo is BreakableEntityInfo)
    //     {
    //         bInfo = eInfo as BreakableEntityInfo;
    //     }
    // }
    
    public void Break()
    {
        GameState.GetInstance()._ParticleSystem.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        GameState.GetInstance()._ParticleSystem.Play();

        foreach (var drop in AssetLoader.GetData(localID).dropData.dropDataList)
        {
            int dropCount = drop.GetDropCount();
            for (int i = 0; i < dropCount; i++)
            {
                Vector2 dropDirection = Random.insideUnitCircle.normalized * 3;
                print("Trees drop " + drop.id);
                // get the item info associated with each drop
                GameObject collectible = GameEntity.GenerateGameEntity(drop.id, this.transform.position);
                var ce = collectible.AddComponent<CollectableEntity>();

                print("This should be 2 or 3: " + drop.id);
                ce.SetCollectible(drop.GetItemStack());
                ce.Move(dropDirection);
            }
        }

        GameObject.Destroy(this.gameObject);
    }

    public void Damage(int damagePoints, int power)
    {
        if (power >= bInfo.minPower)
        {
            damageStatus -= damagePoints;
            if (damageStatus <= 0)
                Break();
        }
    }
}