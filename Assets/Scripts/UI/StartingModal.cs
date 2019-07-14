using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using TMPro;
using UnityEngine;

namespace NotReaper.UI {


    public class StartingModal : MonoBehaviour {

        public GameObject startingModal;

        public Timeline timeline;

        public TextMeshProUGUI recentText;

        void Start() {
            EditorInput.inUI = true;
            startingModal.SetActive(true);
            recentText.text = "OPEN RECENT: coming soon";
        }

        public void Deactivate() {
            startingModal.SetActive(false);
            EditorInput.inUI = false;
        }



        public void LoadExistingAudica() {
            timeline.LoadAudicaFile();
            Deactivate();
        }

        public void NewAudica() {
            
            Deactivate();
        }

        public void LoadRecentAudica() {

            Deactivate();
        }


        public void Quit() {
            Application.Quit();
        }


    }
}