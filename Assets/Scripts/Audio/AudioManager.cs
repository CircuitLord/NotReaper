using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper {


    public class AudioManager : MonoBehaviour {
        public AudioSource main;

        public Timeline timeline;

        public Slider mainSlider;
        public Slider sustainSlider;

        private void Start() {
            SetDefaultVolumes();
        }

        public void SetDefaultVolumes() {
            mainSlider.value = UserPrefsManager.mainVol;
            sustainSlider.value = UserPrefsManager.sustainVol;
        }


        public void SetMainVol() {
            main.volume = mainSlider.value;
        }


        public void SetSustainVol() {
            timeline.sustainVolume = sustainSlider.value;
        }

    }
}