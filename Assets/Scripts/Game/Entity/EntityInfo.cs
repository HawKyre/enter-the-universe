using System;
using UnityEngine;

public class EntityInfo {
    private Sprite entitySprite;
    private EntityType entityType;

    public Sprite _EntitySprite { get => entitySprite; set => entitySprite = value; }
    public EntityType _EntityType { get => entityType; set => entityType = value; }
}

[Flags]
public enum EntityType
{
    BREAKABLE = 1,
    COLLECTIBLE = 2,
    INTERACTABLE = 4,
    HOVERABLE = 8,
    ATTACKABLE = 16
}