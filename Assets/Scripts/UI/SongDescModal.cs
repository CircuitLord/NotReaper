using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using TMPro;
using UnityEngine;
using NotReaper.IO;

namespace NotReaper.UI {


    public class SongDescModal : MonoBehaviour {
        public TMP_InputField songID;
        public TMP_InputField songTitle;
        public TMP_InputField artist;
        public TMP_InputField bpm;
        public TMP_InputField songEndEvent;
        public TMP_InputField mapper;
        public TMP_InputField offset;
        
        public GameObject songDescModal;

        public Timeline timeline;

        bool isNewSong = false;
        string oggPath;


        public void OpenSongDesc() {
            songID.text = Timeline.audicaFile.desc.songID;
            songTitle.text = Timeline.audicaFile.desc.title;
            artist.text = Timeline.audicaFile.desc.artist;
            bpm.text = Timeline.audicaFile.desc.tempo.ToString();
            songEndEvent.text = Timeline.audicaFile.desc.songEndEvent;
            mapper.text = Timeline.audicaFile.desc.mapper;
            offset.text = Timeline.audicaFile.desc.offset.ToString();


            songDescModal.SetActive(true);
            EditorInput.inUI = true;

        }

        public void NewSongDesc(string oggPath) {

            isNewSong = true;
            songID.text = "";
            songTitle.text = "";
            artist.text = "";
            bpm.text = "";
            songEndEvent.text = "event:/song_end/song_end_A";
            mapper.text = "";
            offset.text = "0";
            songDescModal.SetActive(true);
            EditorInput.inUI = true;
        }


        public void SaveSongDesc() {
            int intBPM = 0;
            Int32.TryParse(bpm.text, out intBPM);

            int intOffset = 0;
            Int32.TryParse(offset.text, out intOffset);


            if (isNewSong) {
                
                //Check all fields are filled
                if (songID.text == "") return;
                if (songTitle.text == "") return;
                if (artist.text == "") return;
                if (intBPM == 0) return;
                if (songEndEvent.text == "") return;
                if (mapper.text == "") return;
                

                isNewSong = false;

                string path = AudicaGenerator.Generate(oggPath, songID.text, songTitle.text, artist.text, intBPM, songEndEvent.text, mapper.text, intOffset);
                timeline.LoadAudicaFile(false, path);
                

            } else {
                timeline.UpdateSongDesc(songID.text, songTitle.text, intBPM, songEndEvent.text, mapper.text, intOffset);

            }

            songDescModal.SetActive(false);

            EditorInput.inUI = false;


        }


    }
}