using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDictionary", menuName = "Enter The Dimension/ItemDictionary", order = 0)]
public class ItemDictionary : ScriptableObject
{
    private static string basePath = "Assets/Textures/Items";
    public Dictionary<int, ItemInfo> iDict;

    private void Awake() {
        LoadDictionary_v0();
    }

    public bool LoadDictionary_v0()
    {
        iDict = new Dictionary<int, ItemInfo>();

        Sprite woodSprite = (Sprite) AssetDatabase.LoadAssetAtPath($"{basePath}/Overworld/PrototypePlains/prototypeWood.png", typeof(Sprite));

        ItemInfo i = new ItemInfo();
        i.itemSprite = woodSprite;

        iDict.Add(1, i);

        Sprite stickSprite = (Sprite) AssetDatabase.LoadAssetAtPath($"{basePath}/Overworld/PrototypePlains/prototypeStick.png", typeof(Sprite));

        ItemInfo i2 = new ItemInfo();
        i2.itemSprite = stickSprite;

        iDict.Add(2, i2);

        return true;
    }
}
