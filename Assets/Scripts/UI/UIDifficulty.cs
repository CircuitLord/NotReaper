using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NotReaper.Managers;

namespace NotReaper.UI {



    public class UIDifficulty : MonoBehaviour {
        public DifficultyManager difficultyManager;
        public Button expert;
        public Button advanced;
        public Button standard;
        public Button easy;

        public GameObject warningWindow;
        public TextMeshProUGUI warningText;
        
        

        private string ogDiff = "";
        private string newDiff = "";



        public void ApplyDifficultyToOther() {
            Debug.Log("Applying " + ogDiff + " cues to " + newDiff);

            int origin = -1;
            int dest = -1;

            if (ogDiff == "expert") origin = 0;
            if (ogDiff == "advanced") origin = 1;
            if (ogDiff == "standard") origin = 2;
            if (ogDiff == "easy") origin = 3;

            if (newDiff == "expert") dest = 0;
            if (newDiff == "advanced") dest = 1;
            if (newDiff == "standard") dest = 2;
            if (newDiff == "easy") dest = 3;

            difficultyManager.CopyToOtherDifficulty(origin, dest);
        }


        public void DifficultyComingFrom(string difficulty) {

            ogDiff = difficulty;

            expert.interactable = true;
            advanced.interactable = true;
            standard.interactable = true;
            easy.interactable = true;

            if (difficulty == "expert") expert.interactable = false;
            if (difficulty == "advanced") advanced.interactable = false;
            if (difficulty == "standard") standard.interactable = false;
            if (difficulty == "easy") easy.interactable = false;


        }

        public void Confirm(string newDifficulty) {
            newDiff = newDifficulty;
            warningText.SetText("WARNING: This will replace all cues in " + newDiff + " with the " + ogDiff + " cues.");
            warningWindow.SetActive(true);




        }

        public void WarningCancel() {
            warningWindow.SetActive(false);
        }



        public void Cancel() {
            gameObject.SetActive(false);
        }


    }

}