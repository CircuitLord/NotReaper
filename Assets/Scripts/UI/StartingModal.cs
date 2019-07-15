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
        public SongDescModal songDescModal;

        void Start() {
            EditorInput.inUI = true;
            recentText.text = "OPEN RECENT: " + PlayerPrefs.GetString("recentFile", "none");
            startingModal.SetActive(true);
        }

        public void Deactivate() {
            EditorInput.inUI = false;
            startingModal.SetActive(false);
        }


        public void LoadExistingAudica() {
            timeline.LoadAudicaFile();
            Deactivate();
        }

        public void NewAudica() {
            Deactivate();
            songDescModal.NewSongDesc();
        }

        public void LoadRecentAudica() {
            timeline.LoadAudicaFile(true);
            Deactivate();
        }


        public void Quit() {
            Application.Quit();
        }


    }
}