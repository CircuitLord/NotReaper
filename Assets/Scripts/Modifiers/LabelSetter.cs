using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LabelSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Slider slider;
    public void SetText(string text)
    {
        label.text = text;
    }

    public void SetMinValue(float min)
    {
        slider.minValue = min;
    }
    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
    }

    public string GetText()
    {
        return inputField.text;
    }
}
