﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public static ZoneState GenerateNewZone_v0(long seed, Vector2Int zoneIndex)
    {
        return PrototypeTerrainGenerator.GenerateZone(seed, zoneIndex);
    }
}

