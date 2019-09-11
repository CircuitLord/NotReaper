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


        IEnumerator GetAudioClip(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					audioFile = DownloadHandlerAudioClip.GetContent(www);

                    int bpm = 120;

                    Int32.TryParse(bpmInput.text, out bpm);

                    timeline.LoadTimingMode(audioFile);


					yield break;


				}
			}
		}

        //Tick to MS ["tick"] / 480 * tempo / 1000000
        //ms to tick is = MS * (1920 / (60000 / BPM * 4))

        private void Start() {

        }

        public void ExportOGG() {

        }


        public void SelectAudioFile() {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("ogg", Path.Combine(Application.persistentDataPath), "ogg", false);

            if (paths[0] == null) return;

            StartCoroutine(GetAudioClip($"file://" + paths[0]));
            name.text = paths[0];
        }


        
        public void ApplyValues() {

            if (!timeline.paused) {
                timeline.TogglePlayback();
            }

            int bpm = 120;

            Int32.TryParse(bpmInput.text, out bpm);
            if (bpm == 0) bpm = 120;

            int offset = 0;
            Int32.TryParse(offsetInput.text, out offset);

            timeline.SetTimingModeStats(bpm, offset);

            Timeline.time = 0;
            timeline.SafeSetTime();
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