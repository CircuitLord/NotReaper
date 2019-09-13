using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI {


    public class PauseMenu : MonoBehaviour {
        
        public EditorInput editorInput;
        public Timeline timeline;
        public Button saveButton;
        
        public List<Image> lColorLines = new List<Image>();
        public List<Image> rColorLines = new List<Image>();

        public List<Image> sliderFadeBG = new List<Image>();
        
        public CanvasGroup window;
        public Image BG;

        public bool isOpened = false;


        private void Start() {


            
        }

        public void NewAudica() {
            ClosePauseMenu();

            editorInput.SelectMode(EditorMode.Timing);
            
        }

        public void Open() {
            bool loaded = timeline.LoadAudicaFile(false);
            if (loaded) ClosePauseMenu();
            editorInput.FigureOutIsInUI();
            
        }

        public void OpenRecent() {
            bool loaded = timeline.LoadAudicaFile(true);
            if (loaded) ClosePauseMenu();
            editorInput.FigureOutIsInUI();
        }

        public void OpenPauseMenu() {
            isOpened = true;

            if (Timeline.audicaLoaded) {
                saveButton.interactable = true;
            } else {
                saveButton.interactable = false;
            }

            BG.gameObject.SetActive(true);
            window.gameObject.SetActive(true);


        }

        public void ClosePauseMenu() {
            isOpened = false;

            BG.gameObject.SetActive(false);
            window.gameObject.SetActive(false);


        }


        public void LoadUIColors() {

            Color lColor = NRSettings.config.leftColor;
            Color rColor = NRSettings.config.rightColor;

            foreach (Image img in lColorLines) {
                img.color = lColor;
            }

            foreach (Image img in rColorLines) {
                img.color = rColor;
            }

            foreach (Image img in sliderFadeBG) {
                img.color = new Color(rColor.r, rColor.g, rColor.b, 0.5f);
            }
        }


    }

}