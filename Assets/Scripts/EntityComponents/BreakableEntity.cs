using System.Collections.Generic;
using UnityEngine;

public class BreakableEntity : MonoBehaviour
{
    private int damageStatus;
    private int ID;

    public int hitPoints;
    public int minPower;
    // public List<ToolType> restrictedTools;

    public string dropDataString;
    private List<ItemDropData> dropData;

    private void Awake()
    {
        ID = this.gameObject.GetComponent<GameEntity>().ID;

        // Get the sprite's physics shape
        List<Vector2> physicsShapeList = new List<Vector2>();
        this.gameObject.GetComponent<SpriteRenderer>().sprite.GetPhysicsShape(0, physicsShapeList);
        
        // Assign it to the polygon collider
        var pc2d = this.gameObject.AddComponent<PolygonCollider2D>();
        pc2d.points = physicsShapeList.ToArray();

        damageStatus = hitPoints;
        dropData = DropData.GetDropData(dropDataString);
    }
    
    public void Break()
    {
        GameState.GetInstance()._ParticleSystem.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        GameState.GetInstance()._ParticleSystem.Play();

        foreach (var drop in dropData)
        {
            int dropCount = drop.GetDropCount();
            for (int i = 0; i < dropCount; i++)
            {
                Vector2 dropDirection = Random.insideUnitCircle.normalized * 3;
                print("Trees drop " + drop.id);
                // get the item info associated with each drop
                GameObject collectible = GameEntity.GenerateGameItem(drop.id, this.transform.position, new ItemStack(drop.id, 1));
                var ce = collectible.GetComponent<CollectibleEntity>();
                ce.Move(dropDirection);

                GameState.GetInstance()._ZoneState.AddItem(ce);
            }
        }

        GameObject.Destroy(this.gameObject);
    }

    public void Damage(int damagePoints, int power)
    {
        if (power >= minPower)
        {
            damageStatus -= damagePoints;
            if (damageStatus <= 0)
                Break();
        }
    }
}