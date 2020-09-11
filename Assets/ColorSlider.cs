
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image display;
    [SerializeField] TextMeshProUGUI text;
    public Color color;

    public UnityEvent onColorsSet;

    void Start()
    {
        slider.onValueChanged.AddListener(new UnityAction<float>((value) => { UpdateDisplay(value); }));
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        display.color = color;
        Vector3 hsv;
        Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);
        slider.value = hsv.x;
    }

    public void UpdateDisplay(float value)
    {
        Vector3 rgb;
        Color newColor = Color.HSVToRGB(slider.value, 0.55f, 1f);
        SetColor(newColor);
    }
}
