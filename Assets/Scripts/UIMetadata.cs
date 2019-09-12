using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace NotReaper.UI {


    public class UIMetadata : MonoBehaviour {

        public Image BG;
        public CanvasGroup window;

        public Image titleLine;
        public List<Image> inputBoxLines = new List<Image>();
        public List<Image> inputBoxLinesCover = new List<Image>();
        //public List<Image> inputBoxLines = new List<Image>();



        public TMP_InputField titleField;
        public TMP_InputField artistField;
        public TMP_InputField mapperField;
        public TMP_InputField bpmField;
        public TMP_InputField offsetField;




        
        public void UpdateUIValues() {

            if (!Timeline.audicaLoaded) return;

            if (Timeline.audicaFile.desc.title != null) titleField.text = Timeline.audicaFile.desc.title;
            if (Timeline.audicaFile.desc.artist != null) artistField.text = Timeline.audicaFile.desc.artist;
            if (Timeline.audicaFile.desc.mapper != null) mapperField.text = Timeline.audicaFile.desc.mapper;
            if (Timeline.audicaFile.desc.offset != 0) offsetField.text = Timeline.audicaFile.desc.offset.ToString();
            if (Timeline.audicaFile.desc.tempo != 0) bpmField.text = Timeline.audicaFile.desc.tempo.ToString();
        }
        



        public IEnumerator FadeIn() {
            

            if (!Timeline.audicaLoaded) yield break;

            float fadeDuration = (float)NRSettings.config.UIFadeDuration;

            
            titleLine.color = NRSettings.config.leftColor;
            
            //Set colors
            foreach (Image img in inputBoxLines) {
                img.color = NRSettings.config.leftColor;

            }

            foreach (Image img in inputBoxLinesCover) {
                img.color = NRSettings.config.rightColor;

            }

            UpdateUIValues();

            BG.DOFade(1.0f, fadeDuration / 2f);

            yield return new WaitForSeconds(fadeDuration / 4f);

            DOTween.To(x => window.alpha = x, 0.0f, 1.0f, fadeDuration / 2f);

            yield break;
        }

        public IEnumerator FadeOut() {

            float fadeDuration = (float)NRSettings.config.UIFadeDuration;

            DOTween.To(x => window.alpha = x, 1.0f, 0.0f, fadeDuration / 4f);

            BG.DOFade(0.0f, fadeDuration / 2f);

            yield return new WaitForSeconds(fadeDuration / 2f);

            this.gameObject.SetActive(false);

            yield break;
        }

        
    }

}