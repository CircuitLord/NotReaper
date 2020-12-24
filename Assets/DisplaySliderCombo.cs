using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySliderCombo : MonoBehaviour
{
    public float value
    {
        set
        {
            var slider = sliderObject.GetComponent<Slider>();
            slider.value = value;
            displayTextObject.GetComponent<TextMeshProUGUI>().text = value.ToString("F0") + " db";
        }
    }

    public GameObject displayTextObject;
    public GameObject sliderObject;

    public event Action<float> OnValueChanged = delegate { };


    // Start is called before the first frame update
    void Start()
    {
        var slider = sliderObject.GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });

        displayTextObject.GetComponent<TextMeshProUGUI>().text = slider.value.ToString("F0") + " db";
    }

    public void SliderValueChangeCheck()
    {
        var slider = sliderObject.GetComponent<Slider>();
        float value = slider.value;

        displayTextObject.GetComponent<TextMeshProUGUI>().text = value.ToString("F0") + " db";
        OnValueChanged(value);
    }
}
