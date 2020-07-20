using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapObject", menuName = "Enter The Dimension/MapObject", order = 0)]
public class MapObject : ScriptableObject
{
    private ZoneMap zoneMap;

    public ZoneMap ZoneMap { get => zoneMap; set => zoneMap = value; }

    public void InstantiateZoneMap(int width, int height)
    {
        if (zoneMap == null)
        {
            zoneMap = new ZoneMap(width, height);
        }
    }
}
