using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using NotReaper.IO;
using NotReaper.UI;
using NotReaper.UserInput;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NotReaper.Timing {


    public class UITiming : MonoBehaviour {


        [Header("UI Elements")]
        public Button genAudicaButton;
        public Button applyButton;
        public Button selectSongButton;
        public TMP_InputField bpmInput;
        public TMP_InputField offsetInput;
        public TextMeshProUGUI nameText;
        public TMP_InputField songNameInput;
        public TMP_InputField mapperInput;


        [Header("Extras")]
        public Image BG;
        public CanvasGroup window;
        public EditorInput editorInput;


        private AudioClip audioFile;
        public Timeline timeline;


        public int offset = 0;
        public double bpm = 0;
        public string loadedSong;

        public string songName = "";
        public string mapperName = "";


        TrimAudio trimAudio = new TrimAudio();


        public void SelectAudioFile() {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("ogg", Path.Combine(Application.persistentDataPath), "ogg", false);

            if (paths[0] == null) return;

            StartCoroutine(GetAudioClip($"file://" + paths[0]));
            nameText.text = paths[0];
            loadedSong = paths[0];


        }


        public void ApplyValues() {

            if (!timeline.paused) {
                timeline.TogglePlayback();
            }

            Double.TryParse(bpmInput.text, out bpm);
            Int32.TryParse(offsetInput.text, out offset);

            timeline.SetTimingModeStats(bpm, offset);

            mapperName = RemoveSpecialCharacters(mapperInput.text);
            songName = RemoveSpecialCharacters(songNameInput.text);

            CheckAllUIFilled();

        }

        public void GenerateOgg() {
            trimAudio.SetAudioLength(loadedSong, Path.Combine(Application.streamingAssetsPath, "FFMPEG", "output.ogg"), offset, bpm);
            string path = AudicaGenerator.Generate(Path.Combine(Application.streamingAssetsPath, "FFMPEG", "output.ogg"), (songName + "-" + mapperName), (songName + "-" + mapperName), "artist", bpm, "event:/song_end/song_end_C#", mapperName, 0);
            timeline.LoadAudicaFile(false, path);
            editorInput.SelectMode(EditorMode.Compose);    

            applyButton.interactable = false;
            genAudicaButton.interactable = false;
            selectSongButton.interactable = false;
            
        }

        public void CheckAllUIFilled() {
            if (bpm != 0 && loadedSong != "" && mapperName != "" && songName != "") {
                genAudicaButton.interactable = true;
            } else {
                genAudicaButton.interactable = false;
            }
        }

        public IEnumerator FadeIn() {


            float fadeDuration = (float) NRSettings.config.UIFadeDuration;



            BG.DOFade(1.0f, fadeDuration / 2f);

            yield return new WaitForSeconds(fadeDuration / 4f);

            DOTween.To(x => window.alpha = x, 0.0f, 1.0f, fadeDuration / 2f);

            yield break;
        }

        public IEnumerator FadeOut() {

            float fadeDuration = (float) NRSettings.config.UIFadeDuration;

            DOTween.To(x => window.alpha = x, 1.0f, 0.0f, fadeDuration / 4f);

            BG.DOFade(0.0f, fadeDuration / 2f);

            yield return new WaitForSeconds(fadeDuration / 2f);

            this.gameObject.SetActive(false);

            yield break;
        }


        IEnumerator GetAudioClip(string uri) {
            using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
                yield return www.SendWebRequest();

                if (www.isNetworkError) {
                    Debug.Log(www.error);
                } else {
                    audioFile = DownloadHandlerAudioClip.GetContent(www);

                    Double.TryParse(bpmInput.text, out bpm);

                    timeline.LoadTimingMode(audioFile);


                    yield break;


                }
            }
        }

        public string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }

}