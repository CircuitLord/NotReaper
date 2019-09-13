using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI {

    public class UIMetadata : MonoBehaviour {

        public Image BG;
        public CanvasGroup window;

        public Image titleLine;
        public List<Image> inputBoxLines = new List<Image>();
        public List<Image> inputBoxLinesCover = new List<Image>();


        public TMP_InputField titleField;
        public TMP_InputField artistField;
        public TMP_InputField mapperField;


        public GameObject selectDiffWindow;

        public Button generateDiff;

        public int selectedDiff;



        public void UpdateUIValues() {

            if (!Timeline.audicaLoaded) return;

            if (Timeline.audicaFile.desc.title != null) titleField.text = Timeline.audicaFile.desc.title;
            if (Timeline.audicaFile.desc.artist != null) artistField.text = Timeline.audicaFile.desc.artist;
            if (Timeline.audicaFile.desc.mapper != null) mapperField.text = Timeline.audicaFile.desc.mapper;

            ChangeSelectedDifficulty(0);
        }


        public void TryCopyCuesToOther() {
            selectDiffWindow.SetActive(true);

            switch (selectedDiff) {
                case 0:
                    selectDiffWindow.GetComponent<UIDifficulty>().DifficultyComingFrom("expert");
                    break;

                case 1:
                    selectDiffWindow.GetComponent<UIDifficulty>().DifficultyComingFrom("advanced");
                    break;

                case 2:
                    selectDiffWindow.GetComponent<UIDifficulty>().DifficultyComingFrom("standard");
                    break;

                case 3:
                    selectDiffWindow.GetComponent<UIDifficulty>().DifficultyComingFrom("easy");
                    break;

            }
        }

        public void ChangeSelectedDifficulty(int index) {

            selectedDiff = index;


            switch (index) {
                case 0:
                    if (Timeline.audicaFile.diffs.expert.cues == null) {
                        generateDiff.interactable = true;
                    } else {
                        generateDiff.interactable = false;
                    }
                    break;

                case 1:
                    if (Timeline.audicaFile.diffs.advanced.cues == null) {
                        generateDiff.interactable = true;
                    } else {
                        generateDiff.interactable = false;
                    }
                    break;

                case 2:
                    if (Timeline.audicaFile.diffs.standard.cues == null) {
                        generateDiff.interactable = true;
                    } else {
                        generateDiff.interactable = false;
                    }
                    break;

                case 3:
                    if (Timeline.audicaFile.diffs.easy.cues == null) {
                        generateDiff.interactable = true;
                    } else {
                        generateDiff.interactable = false;
                    }
                    break;


            }

        }


        public IEnumerator FadeIn() {


            if (!Timeline.audicaLoaded) yield break;

            float fadeDuration = (float) NRSettings.config.UIFadeDuration;


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

            float fadeDuration = (float) NRSettings.config.UIFadeDuration;

            DOTween.To(x => window.alpha = x, 1.0f, 0.0f, fadeDuration / 4f);

            BG.DOFade(0.0f, fadeDuration / 2f);

            yield return new WaitForSeconds(fadeDuration / 2f);

            this.gameObject.SetActive(false);

            yield break;
        }


    }

}