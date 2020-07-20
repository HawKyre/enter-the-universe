using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityDictionary", menuName = "Enter The Dimension/EntityDictionary", order = 0)]
public class EntityDictionary : ScriptableObject {

    private static string basePath = "Assets/Textures/Blocks";
    public Dictionary<int, EntityInfo> eDict;

    private void Awake() {
        LoadDictionary_v0();
    }

    public bool LoadDictionary_v0()
    {
        eDict = new Dictionary<int, EntityInfo>();

        Sprite prototypeTreeSprite = (Sprite) AssetDatabase.LoadAssetAtPath($"{basePath}/Overworld/PrototypePlains/prototypeTree.png", typeof(Sprite));
        BreakableEntityInfo bInfo = new BreakableEntityInfo();
        bInfo._EntitySprite = prototypeTreeSprite;
        bInfo._MinPower = 0;
        bInfo._MaxHealth = 5;
        bInfo._EntityType = EntityType.BREAKABLE;

        List<ItemDropInfo> treeDrops = new List<ItemDropInfo>();
        treeDrops.Add(new ItemDropInfo(1, new Range(2, 4)));
        treeDrops.Add(new ItemDropInfo(2, new Range(1, 1)));

        bInfo._ItemDropInfo = treeDrops;

        eDict.Add(1, bInfo);

        return true;
    }
}

