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
    [SerializeField] private Toggle toggle;
    public void SetLabelText(string text)
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

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public string GetText()
    {
        return inputField.text;
    }

    public void SetInputText(string text)
    {
        inputField.text = text;
    }

    public void SetToggleState(bool on)
    {
        toggle.isOn = on;
    }
}
