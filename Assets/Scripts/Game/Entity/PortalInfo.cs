using UnityEngine;

public class PortalInfo
{
    public GameEntityInfo entityInfo;
    public SVector3Int nextZone;
    public Direction directionToGo;
}

public enum Direction
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT,
    DIM_CHANGE = -1
}