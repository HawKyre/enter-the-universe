using System.Collections.Generic;
using UnityEngine;

public class BreakableEntity : GameEntity
{
    private int damageStatus;
    private BreakableEntityInfo bInfo;

    public int DamageStatus { get => damageStatus; protected set => damageStatus = value; }

    public BreakableEntity(int entityID, Vector3 gameEntityPos) : base(entityID, gameEntityPos)
    {
        // We can assume that 
        EntityInfo eInfo;
        GameState.GetInstance()._EntityDictionary.eDict.TryGetValue(entityID, out eInfo);
        
        if (eInfo is BreakableEntityInfo)
        {
            bInfo = eInfo as BreakableEntityInfo;
        }
    }
    
    public void Break()
    {
        GameState.GetInstance()._ParticleSystem.transform.position = new Vector3(EntityObject.transform.position.x, EntityObject.transform.position.y, 0);
        GameState.GetInstance()._ParticleSystem.Play();

        foreach (var drop in ((BreakableEntityInfo) GameState.GetInstance()._EntityDictionary.eDict[ID])._ItemDropInfo)
        {
            int dropCount = drop.count.GetInRange();
            for (int i = 0; i < dropCount; i++)
            {
                Vector2 dropDirection = Random.insideUnitCircle.normalized * 3;

                // get the item info associated with each drop
                ItemStackEntity iStack = new ItemStackEntity(drop.itemID, 1, entityObject.transform.position);
                iStack.Move(dropDirection);
            }
        }

        GameObject.Destroy(entityObject.gameObject);
    }

    public void Damage(int damagePoints, int power)
    {
        if (power >= bInfo._MinPower)
        {
            damageStatus -= damagePoints;
            if (damageStatus <= 0)
                Break();
        }
    }
}