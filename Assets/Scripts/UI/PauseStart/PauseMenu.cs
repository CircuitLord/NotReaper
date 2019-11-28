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
        public Button newAudicaButton;

        public CanvasGroup window;
        public Image BG;

        public bool isOpened = false;


        public void Start() {
            var t = transform;
            var position = t.localPosition;
            t.localPosition = new Vector3(0, position.y, position.z);
        }

        public void NewAudica() {
            ClosePauseMenu();

            editorInput.SelectMode(EditorMode.Timing);
            Timeline.inTimingMode = true;

            newAudicaButton.interactable = false;
            
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





    }

}