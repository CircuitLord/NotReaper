using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTips : MonoBehaviour
{
    public static ToolTips I;
    public TextMeshProUGUI label;

    void Awake()
    {
        if (I == null) I = this;
        label.text = "";
    }

    public void SetText(string text)
    {
        label.text = text.Replace("[", "<color=#FDA50F>").Replace("]", "</color>");
    }

}
