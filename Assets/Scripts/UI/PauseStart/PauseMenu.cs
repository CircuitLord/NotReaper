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
        [SerializeField] RecentPanel recentPanel;

        public CanvasGroup window;
        public Image BG;

        public bool isOpened = false;


        public void Start() {
            var t = transform;
            var position = t.localPosition;
            t.localPosition = new Vector3(0, position.y, position.z);
            recentPanel.Show();
        }

        public void NewAudica() {
            ClosePauseMenu();

            editorInput.SelectMode(EditorMode.Timing);
            Timeline.inTimingMode = true;

            //newAudicaButton.interactable = false;
            
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

            newAudicaButton.interactable = !Timeline.audicaLoaded;
            
            BG.gameObject.SetActive(true);
            window.gameObject.SetActive(true);
            recentPanel.Show();

        }

        public void ClosePauseMenu() {
            isOpened = false;

            BG.gameObject.SetActive(false);
            window.gameObject.SetActive(false);


        }





    }

}