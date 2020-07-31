using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public static SZoneState GenerateNewZone_v0(long seed, Vector3Int zoneIndex)
    {
        long usedSeed = seed + GetSeedWithOffset(seed, zoneIndex);
        switch (zoneIndex.z)
        {
            case -1:
                // Prototype dimension
                return PrototypeTerrainGenerator.GenerateZone(usedSeed, zoneIndex);
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
                return PrototypeTerrainGenerator.GenerateZone(usedSeed, zoneIndex);
        }

        return null;
    }

    private static long GetSeedWithOffset(long s, Vector3Int v)
    {
        long seed = s;
        // Get the offset from (x, y), get the shifting from the main seed from (z * 37 + 53) % 64

        // x, y
        seed += Util.ZZtoZ(new Vector2Int(v.x, v.y));

        Debug.Log($"For zone {v} : {seed} and {Util.RotateLeft(seed, ((v.z + 64) * 37 + 53) % 64)} is shift");

        // z
        // seed = Util.RotateLeft(seed, (v.z * 37 + 53) % 64);

        Debug.Log($"Final seed: {seed}");
        return seed;
    }
}

