using System.Collections;
using System.Collections.Generic;
using SFB;
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
            string[] paths = StandaloneFileBrowser.OpenFilePanel("OGG File", Application.dataPath, "ogg", false);
            string oggPath = paths.Length > 0 ? paths[0] : "";
            if (oggPath != "") {
                Deactivate();
                songDescModal.NewSongDesc(oggPath);
            }
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