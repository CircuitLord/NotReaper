using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.ModernUIPack {
    public class HorizontalSelector : MonoBehaviour {
        private TextMeshProUGUI label;
        private TextMeshProUGUI labeHelper;
        private Animator selectorAnimator;

        [Header("SETTINGS")]
        public int index = 0;
        public int defaultIndex = 0;

        [Header("ELEMENTS")]
        public List<string> elements = new List<string>();

        [Header("EVENT")]
        public UnityEvent onValueChanged;

        void Start() {
            selectorAnimator = gameObject.GetComponent<Animator>();
            label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            labeHelper = transform.Find("Text Helper").GetComponent<TextMeshProUGUI>();
            label.text = elements[defaultIndex];
            labeHelper.text = label.text;
        }

        public void UpdateToIndex(int idx) {
            if (labeHelper == null) {
                return;
            }
            
            labeHelper.text = label.text;

            index = idx;
            onValueChanged.Invoke();
            label.text = elements[index];

        }

        public void PreviousClick() {
            labeHelper.text = label.text;

            if (index == 0) {
                index = elements.Count - 1;
            } else {
                index--;
            }

            onValueChanged.Invoke();
            label.text = elements[index];

        }

        public void ForwardClick() {
            labeHelper.text = label.text;

            if ((index + 1) >= elements.Count) {
                index = 0;
            } else {
                index++;
            }

            onValueChanged.Invoke();
            label.text = elements[index];
        }
    }
}