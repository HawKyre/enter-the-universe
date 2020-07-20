using UnityEngine;

public class EntityInfo {
    private Sprite entitySprite;
    private EntityType entityType;

    public Sprite _EntitySprite { get => entitySprite; set => entitySprite = value; }
    public EntityType _EntityType { get => entityType; set => entityType = value; }
}

public enum EntityType
{
    BREAKABLE,
    STATIC
}