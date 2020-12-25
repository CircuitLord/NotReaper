using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NotReaper.Modifier
{
    public class IconTextSetter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetText(string _text)
        {
            text.text = _text;
        }
    }
}

