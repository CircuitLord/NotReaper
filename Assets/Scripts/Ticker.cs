using System.Collections;
using DG.Tweening;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper {


    public class Ticker : MonoBehaviour {

        public LayerMask layermask;
        AudioSource aud;

        public AudioSource kick;
        public AudioSource snare;
        public AudioSource percussion;
        public AudioSource chainStart;
        public AudioSource chainNode;
        public AudioSource melee;

        public AudioSource metronomeTick;

        public Slider volumeSlider;

        private float volume;
        private float noteHitScale;

        private void Start() {
            aud = GetComponent<AudioSource>();
            volume = volumeSlider.value;
            NRSettings.OnLoad(() => {
                noteHitScale = NRSettings.config.noteHitScale;
                volume = (float)NRSettings.config.noteVol;
                volumeSlider.value = volume;
            });
        }

        public void VolumeChange(Slider vol) {
            volume = vol.value;
            NRSettings.config.noteVol = volume;
            NRSettings.SaveSettingsJson();
        }

        private void OnTriggerEnter(Collider other) {
            if (layermask == (layermask | (1 << other.gameObject.layer))) {
                if (other.transform.position.z > -1) {

                    var icon = other.GetComponent<TargetIcon>();
                    DOTween.To((float scale) => {
                        icon.transform.localScale = new Vector3(scale, scale, 1f);
                    }, noteHitScale, 1f, 0.3f).SetEase(Ease.OutCubic);
                }
            }
        }
    }
}