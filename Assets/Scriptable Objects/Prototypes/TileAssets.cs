using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "TileAssets", menuName = "Enter The Dimension/TileAssets", order = 0)]
public class TileAssets : ScriptableObject
{
    public RuleTile groundRuleTile;
    public RuleTile boundsRuleTile;
}
