using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NotReaper.UI {


    public class SongDescModal : MonoBehaviour {
        public TMP_InputField songID;
        public TMP_InputField songTitle;
        public TMP_InputField artist;
        public TMP_InputField bpm;
        public TMP_InputField songEndEvent;
        public TMP_InputField mapper;
        
        public GameObject songDescModal;

        public Timeline timeline;

        public void OpenSongDesc() {
            songID.text = Timeline.audicaFile.desc.songID;
            songTitle.text = Timeline.audicaFile.desc.title;
            artist.text = Timeline.audicaFile.desc.artist;
            bpm.text = Timeline.audicaFile.desc.tempo.ToString();
            songEndEvent.text = Timeline.audicaFile.desc.songEndEvent;
            mapper.text = Timeline.audicaFile.desc.mapper;


            songDescModal.SetActive(true);

        }

        public void NewSongDesc() {
            songID.text = "";
            songTitle.text = "";
            artist.text = "";
            bpm.text = "";
            songEndEvent.text = "event:/song_end/song_end_A";
            mapper.text = "";
            songDescModal.SetActive(true);
        }


        public void SaveSongDesc() {
            songDescModal.SetActive(false);
            int intBPM;
            Int32.TryParse(bpm.text, out intBPM);

            timeline.UpdateSongDesc(songID.text, songTitle.text, intBPM, songEndEvent.text, mapper.text);


        }


    }
}