using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotReaper.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI {

    public class UIMetadata : MonoBehaviour {


        public DifficultyManager difficultyManager;

        public Image BG;
        public CanvasGroup window;

        public Image titleLine;
        public List<Image> inputBoxLines = new List<Image>();
        public List<Image> inputBoxLinesCover = new List<Image>();


        public TMP_InputField titleField;
        public TMP_InputField artistField;
        public TMP_InputField mapperField;

        public TMP_InputField moggSongVolume;


        public GameObject selectDiffWindow;

        public Button generateDiff;
        public Button loadThisDiff;

        private int selectedDiff;
        private int diffPotentiallyGoingDelete = -1;

        public GameObject warningDeleteWindow;

        public TMP_Dropdown diffDropdown;
        public TMP_Dropdown pitchDropdown;



        public void Start() {
            var t = transform;
            var position = t.localPosition;
            t.localPosition = new Vector3(0, position.y, position.z);
        }
        
        public void UpdateUIValues() {

            if (!Timeline.audicaLoaded) return;

            if (Timeline.desc.title != null) titleField.text = Timeline.desc.title;
            if (Timeline.desc.artist != null) artistField.text = Timeline.desc.artist;
            if (Timeline.desc.author != null) mapperField.text = Timeline.desc.author;

            if (Timeline.desc.moggSong != null) moggSongVolume.text = Timeline.audicaFile.mainMoggSong.volume.l.ToString("n2");

            diffDropdown.value = difficultyManager.loadedIndex;
            ChangeSelectedDifficulty(difficultyManager.loadedIndex);
            // Song end pitch event
            switch (Timeline.desc.songEndEvent)
            {
                case "event:/song_end/song_end_nopitch":
                    pitchDropdown.value = 0;
                    break;

                case "event:/song_end/song_end_A":
                    pitchDropdown.value = 1;
                    break;

                case "event:/song_end/song_end_A#":
                    pitchDropdown.value = 2;
                    break;

                case "event:/song_end/song_end_B":
                    pitchDropdown.value = 3;
                    break;

                case "event:/song_end/song_end_C":
                    pitchDropdown.value = 4;
                    break;

                case "event:/song_end/song_end_C#":
                    pitchDropdown.value = 5;
                    break;

                case "event:/song_end/song_end_D":
                    pitchDropdown.value = 6;
                    break;

                case "event:/song_end/song_end_D#":
                    pitchDropdown.value = 7;
                    break;

                case "event:/song_end/song_end_E":
                    pitchDropdown.value = 8;
                    break;

                case "event:/song_end/song_end_F":
                    pitchDropdown.value = 9;
                    break;

                case "event:/song_end/song_end_F#":
                    pitchDropdown.value = 10;
                    break;

                case "event:/song_end/song_end_G":
                    pitchDropdown.value = 11;
                    break;

                case "event:/song_end/song_end_G#":
                    pitchDropdown.value = 12;
                    break;
            }

        }

        public void ApplyValues() {
            if (Timeline.desc == null) return;

            if (String.IsNullOrEmpty(titleField.text)) return; 
            
            Timeline.desc.title = titleField.text;
            Timeline.desc.artist = artistField.text;
            Timeline.desc.author = mapperField.text;
            Timeline.audicaFile.mainMoggSong.SetVolume(float.Parse(moggSongVolume.text));
        }

        public void TryCopyCuesToOther() {
            selectDiffWindow.SetActive(true);

            switch (difficultyManager.loadedIndex) {
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
        //Called when the value is changed the dropdown box for the difficulties
        public void ChangeSelectedDifficulty(int index) {
            if (index == -1) return;

            selectedDiff = index;

            if (difficultyManager.loadedIndex == index) {
                loadThisDiff.interactable = false;
            } else {
                loadThisDiff.interactable = true;
            }

            if (difficultyManager.DifficultyExists(index)) {
                generateDiff.interactable = false;
            } else {
                generateDiff.interactable = true;
                loadThisDiff.interactable = false;
            }
        }

        public void ChangeEndPitch()
        {
            switch (pitchDropdown.value)
            {
                case 0:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_nopitch";
                    break;

                case 1:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_A";
                    break;

                case 2:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_A#";
                    break;

                case 3:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_B";
                    break;

                case 4:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_C";
                    break;

                case 5:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_C#";
                    break;

                case 6:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_D";
                    break;

                case 7:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_D#";
                    break;

                case 8:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_E";
                    break;

                case 9:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_F";
                    break;

                case 10:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_F#";
                    break;

                case 11:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_G";
                    break;

                case 12:
                    Timeline.desc.songEndEvent = "event:/song_end/song_end_G#";
                    break;
            }
        }

        public void TryDeleteDifficulty() {
            string diffName = "";
            if (selectedDiff == 0) diffName = "expert";
            if (selectedDiff == 1) diffName = "advanced";
            if (selectedDiff == 2) diffName = "standard";
            if (selectedDiff == 3) diffName = "easy";
            diffPotentiallyGoingDelete = selectedDiff;
            warningDeleteWindow.GetComponentInChildren<TextMeshProUGUI>().text = String.Format("WARNING: This will remove ALL cues in {0}. Are you SURE you want to do this?", diffName);
            warningDeleteWindow.SetActive(true);
        }

        //After the confirmation message
        public void ActuallyDeleteDifficulty() {
            warningDeleteWindow.SetActive(false);
            difficultyManager.RemoveDifficulty(diffPotentiallyGoingDelete);
            UpdateUIValues();
        }

        public void GenerateDifficulty() {
            difficultyManager.GenerateDifficulty(selectedDiff);
            difficultyManager.LoadDifficulty(selectedDiff, true);
            UpdateUIValues();
        }

        public void LoadThisDiff() {
            difficultyManager.LoadDifficulty(selectedDiff, true);
            UpdateUIValues();
        }


        public IEnumerator FadeIn() {

            if (!Timeline.audicaLoaded) yield break;

            titleLine.color = NRSettings.config.leftColor;

            //Set colors
            foreach (Image img in inputBoxLines) {
                img.color = NRSettings.config.leftColor;
            }

            foreach (Image img in inputBoxLinesCover) {
                img.color = NRSettings.config.rightColor;
            }

            UpdateUIValues();

            BG.gameObject.SetActive(true);
            window.gameObject.SetActive(true);
        }

        public IEnumerator FadeOut() {
            BG.gameObject.SetActive(false);
            window.gameObject.SetActive(false);
            
            ApplyValues();
            
            yield break;
        }


    }

}