using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextSliderCombo : MonoBehaviour
{
    public float value
    {
        set { 
            var slider = sliderObject.GetComponent<Slider>();
            slider.value = value;
            inputFieldObject.GetComponent<TMP_InputField>().text = value.ToString("F2");
        }
    }

    public GameObject inputFieldObject;
    public GameObject sliderObject;

    public event Action<float> OnValueChanged = delegate {};


    // Start is called before the first frame update
    void Start() {
        var slider = sliderObject.GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate {SliderValueChangeCheck(); });

        inputFieldObject.GetComponent<TMP_InputField>().text = slider.value.ToString("F2");
        inputFieldObject.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate {TextValueChangeCheck(); });
    }

    public void SliderValueChangeCheck() {
        var slider = sliderObject.GetComponent<Slider>();
        float value = slider.value;

        inputFieldObject.GetComponent<TMP_InputField>().text = value.ToString("F2");;
        OnValueChanged(value);
    }

    public void TextValueChangeCheck() {
        var text = inputFieldObject.GetComponent<TMP_InputField>().text;
        
        float newValue = sliderObject.GetComponent<Slider>().value;
        float.TryParse(text, out newValue);
        sliderObject.GetComponent<Slider>().value = newValue;

        OnValueChanged(newValue);
    }
}
