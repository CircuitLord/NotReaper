using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextSliderCombo : MonoBehaviour
{
    public float value;

    public GameObject inputFieldObject;
    public GameObject sliderObject;


    // Start is called before the first frame update
    void Start() {
        var slider = sliderObject.GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });

        inputFieldObject.GetComponent<TMP_InputField>().text = slider.value.ToString("F2");
    }

    public void ValueChangeCheck() {
        var slider = sliderObject.GetComponent<Slider>();
        inputFieldObject.GetComponent<TMP_InputField>().text = slider.value.ToString("F2");
    }
}
