using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Michsky.UI.ModernUIPack;

namespace NotReaper.UI {


    public class ProgressSliderColors : MonoBehaviour {

        public UIGradient gradient;

        // Start is called before the first frame update
        void Start() {

            gradient.EffectGradient = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(UserPrefsManager.leftColor, 0), new GradientColorKey(UserPrefsManager.rightColor, 1) } };

        }

    }
}