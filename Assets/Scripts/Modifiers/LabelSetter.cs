using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
namespace NotReaper.Modifier
{
    public class LabelSetter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Slider slider;
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI hint;
        [Header("Color")]
        [SerializeField] private Slider colorSliderHueLeft;
        [SerializeField] private Slider colorSliderHueRight;
        [SerializeField] private Slider colorSliderSaturationLeft;
        [SerializeField] private Slider colorSliderSaturationRight;
        [SerializeField] private SpriteRenderer colorFieldLeft;
        [SerializeField] private SpriteRenderer colorFieldRight;

        public void Start()
        {
            if(inputField != null)
            {
                inputField.onSelect.AddListener(ModifierHandler.Instance.OnInputFocusChange);
                inputField.onDeselect.AddListener(ModifierHandler.Instance.OnInputFocusChange);
            }
        }

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

        public bool GetToggleState()
        {
            return toggle.isOn;
        }

        public void SetHintText(string text)
        {
            hint.text = text;
        }

        public void SetColorSliderLeft(float[] col)
        {
            if (col.Length < 3) col = new float[] { 0f, 0f, 0f };
            Color color = new Color(col[0], col[1], col[2]);
            float h, s;
            Color.RGBToHSV(color, out h, out s, out _);
            colorSliderHueLeft.value = h;
            colorSliderSaturationLeft.value = s;
        }

        public void SetColorSliderRight(float[] col)
        {
            if (col.Length < 3) col = new float[] { 0f, 0f, 0f };
            Color color = new Color(col[0], col[1], col[2]);
            float h, s;
            Color.RGBToHSV(color, out h, out s, out _);
            colorSliderHueRight.value = h;
            colorSliderSaturationRight.value = s;
        }

        public void SetMinMaxColorSliders(float min, float max)
        {
            colorSliderHueLeft.minValue = min;
            colorSliderHueRight.minValue = min;

            colorSliderSaturationLeft.minValue = min;
            colorSliderSaturationRight.minValue = min;


            colorSliderHueLeft.maxValue = max;
            colorSliderHueRight.maxValue = max;

            colorSliderSaturationLeft.maxValue = max;
            colorSliderSaturationRight.maxValue = max;           
        }

        public float[] GetLeftColor()
        {
            Color c = Color.HSVToRGB(colorSliderHueLeft.value, colorSliderSaturationLeft.value, 1f);
            return new float[] { c.r, c.g, c.b };
        }

        public float[] GetRightColor()
        {
            Color c = Color.HSVToRGB(colorSliderHueRight.value, colorSliderSaturationRight.value, 1f);
            return new float[] { c.r, c.g, c.b };
        }
    }
}

