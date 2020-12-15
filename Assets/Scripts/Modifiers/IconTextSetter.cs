using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IconTextSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string _text)
    {
        text.text = _text;
    }
}
