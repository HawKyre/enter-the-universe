
using UnityEngine;

public class UIItem
{
    public int id;
    public int count;
    public GameObject uiObject;

    public UIItem(int id, int count, GameObject uiObject)
    {
        this.id = id;
        this.count = count;
        this.uiObject = uiObject;
    }
}