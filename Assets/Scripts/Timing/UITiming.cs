using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using NotReaper.Metronome;
using SFB;
using System.IO;

namespace NotReaper.Timing {


    public class UITiming : MonoBehaviour {

        public Image BG;
        public CanvasGroup window;

        public UnityMetronome metronome;

        public TextMeshProUGUI name;

        public TMP_InputField bpmInput;
        public TMP_InputField offsetInput;

        public Button playPause;

        public AudioClip audioFile;

        public Timeline timeline;


        public int offset = 0;
        public double bpm = 120;
        public string loadedSong;


        TrimAudio trimAudio = new TrimAudio();


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

        





        public void SelectAudioFile() {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("ogg", Path.Combine(Application.persistentDataPath), "ogg", false);

            if (paths[0] == null) return;

            StartCoroutine(GetAudioClip($"file://" + paths[0]));
            name.text = paths[0];
            loadedSong = paths[0];
        }


        
        public void ApplyValues() {

            if (!timeline.paused) {
                timeline.TogglePlayback();
            }

            Double.TryParse(bpmInput.text, out bpm);
            Int32.TryParse(offsetInput.text, out offset);

            timeline.SetTimingModeStats(bpm, offset);

        }

        public void GenerateOgg() {
            if (loadedSong == null) return;
            trimAudio.SetAudioLength(loadedSong, Path.Combine(Application.streamingAssetsPath, "FFMPEG", "output2.ogg") , offset, bpm);
        }

        public void UpdateUIValues() {

            
        }

        public IEnumerator FadeIn() {


            float fadeDuration = (float) NRSettings.config.UIFadeDuration;


            UpdateUIValues();

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
    }

}