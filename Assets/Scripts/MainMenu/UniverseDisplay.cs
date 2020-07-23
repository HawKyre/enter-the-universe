using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UniverseDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    
    public void SetupDisplay(string name, Vector2 anchoredPos)
    {
        nameText.text = name;
        var rt = this.GetComponent<RectTransform>();
        rt.anchoredPosition3D = anchoredPos;
        rt.localScale = Vector3.one;
        rt.offsetMin = new Vector2(0, rt.offsetMin.y);
        rt.offsetMax = new Vector2(0, anchoredPos.y);
    }
}
