using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public static SZoneState GenerateNewZone_v0(long seed, Vector3Int zoneIndex)
    {
        switch (zoneIndex.z)
        {
            case -1:
                // Prototype dimension
                return PrototypeTerrainGenerator.GenerateZone(seed, zoneIndex);
            case 0:
                // Overworld dimension
                break;
            case 1:
                // Light dimension
                break;
            case 2:
                // Hell dimension
                break;
            default:
                return PrototypeTerrainGenerator.GenerateZone(seed, zoneIndex);
        }

        return null;
    }
}

