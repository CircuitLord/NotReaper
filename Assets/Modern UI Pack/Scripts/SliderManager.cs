using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class SliderManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("TEXTS")]
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI popupValueText;

        [Header("SETTINGS")]
        public bool usePercent = false;
        public bool showValue = true;
        public bool showPopupValue = true;
        public bool useRoundValue = false;

        private Slider mainSlider;
        private Animator sliderAnimator;

        void Start()
        {
            mainSlider = this.GetComponent<Slider>();
            sliderAnimator = this.GetComponent<Animator>();

            if (showValue == false)
            {
                valueText.enabled = false;
            }

            if (showPopupValue == false)
            {
                popupValueText.enabled = false;
            }
        }

        void Update()
        {
            if (useRoundValue == true)
            {
                if (usePercent == true)
                {
                    valueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString() + "%";
                    popupValueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString() + "%";
                }

                else
                {
                    valueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString();
                    popupValueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString();
                }
            }
            else
            {
                if (usePercent == true)
                {
                    valueText.text = mainSlider.value.ToString("F1") + "%";
                    popupValueText.text = mainSlider.value.ToString("F1") + "%";
                }

                else
                {
                    valueText.text = mainSlider.value.ToString("F1");
                    popupValueText.text = mainSlider.value.ToString("F1");
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (showPopupValue == true)
            {
                sliderAnimator.Play("Value In");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (showPopupValue == true)
            {
                sliderAnimator.Play("Value Out");
            }
        }
    }
}