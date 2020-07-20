using System.Collections.Generic;
using UnityEngine;

public class BreakableEntityInfo : EntityInfo
{
    private int maxHealth;
    private int minPower;
    private List<ItemDropInfo> itemDropInfo;

    public int _MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int _MinPower { get => minPower; set => minPower = value; }
    public List<ItemDropInfo> _ItemDropInfo { get => itemDropInfo; set => itemDropInfo = value; }
}