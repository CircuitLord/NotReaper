using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DG.Tweening;
using NLayer;
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
        public Button selectSongButton;
        public TextMeshProUGUI nameText;
        public TMP_InputField songNameInput;
        public TMP_InputField mapperInput;


        [Header("Extras")]
        public Image BG;
        public CanvasGroup window;
        public EditorInput editorInput;


        private AudioClip audioFile;
        public Timeline timeline;
        public string loadedSong;

        public string songName = "";
        public string mapperName = "";

        public bool skipOffset = true;
        public bool isMp3 = false;

        const int DefaultBPM = 150;


        TrimAudio trimAudio = new TrimAudio();


        Process ffmpeg = new Process();

        private void Start() {
            var t = this.transform;
            var position = t.localPosition;
            t.localPosition = new Vector3(0, position.y, position.z);
            window.alpha = 0;

            string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpeg.exe");

            if ((Application.platform == RuntimePlatform.LinuxEditor) || (Application.platform == RuntimePlatform.LinuxPlayer))
                ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpeg");

            if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer))
                ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpegOSX");

            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ffmpeg.StartInfo.FileName = ffmpegPath;

            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.WorkingDirectory = Path.Combine(Application.streamingAssetsPath, "FFMPEG");
            
        }



        public void SkipOffset(bool yes) {

            skipOffset = yes;

        }
        


        public void SelectAudioFile() {
            var compatible = new[] { new ExtensionFilter("Compatible Audio Types", "mp3", "ogg") }; 
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Select music track", Path.Combine(Application.persistentDataPath), compatible, false);
            var filePath = paths[0];

            if (filePath != null) {
                // if user loads mp3 instead of ogg, do the conversion first
                if (paths[0].EndsWith(".mp3")) {
                    UnityEngine.Debug.Log(String.Format("-y -i \"{0}\" -map 0:a \"{1}\"", paths[0], "converted.ogg"));
                    ffmpeg.StartInfo.Arguments =
                        String.Format("-y -i \"{0}\" -map 0:a \"{1}\"", paths[0], "converted.ogg");
                    ffmpeg.Start();
                    ffmpeg.WaitForExit();
                    filePath = "converted.ogg";

                    isMp3 = true;
                    
                    StartCoroutine(
	                    GetAudioClip($"file://" + Path.Combine(Application.streamingAssetsPath, "FFMPEG", filePath)));
                }

                else {
	                isMp3 = false;
	                StartCoroutine(GetAudioClip(filePath));
                }

                nameText.text = paths[0];
	            loadedSong = paths[0];
            }
        }

        public void Cancel() {
            Timeline.inTimingMode = false;
            editorInput.SelectMode(EditorMode.Compose);
        }
        

        public void ApplyValues() {

            if (!timeline.paused) {
                timeline.TogglePlayback();
            }

            timeline.SetTimingModeStats(Constants.MicrosecondsPerQuarterNoteFromBPM(DefaultBPM), 0);

            mapperName = RemoveSpecialCharacters(mapperInput.text);
            songName = RemoveSpecialCharacters(songNameInput.text);

            CheckAllUIFilled();

        }

        public void GenerateOgg() {
            ApplyValues();
            if(!CheckAllUIFilled()) {
                return;
            }
            

	        string path;

	        if (isMp3 || !skipOffset) {
                trimAudio.SetAudioLength(loadedSong, Path.Combine(Application.streamingAssetsPath, "FFMPEG", "output.ogg"), 0, DefaultBPM, skipOffset);
                path = AudicaGenerator.Generate(Path.Combine(Application.streamingAssetsPath, "FFMPEG", "output.ogg"), (songName + "-" + mapperName), songName, "artist", DefaultBPM, "event:/song_end/song_end_C#", mapperName, 0);
		        
	        }
	        else {
                path = AudicaGenerator.Generate(loadedSong, (songName + "-" + mapperName), songName, "artist", DefaultBPM, "event:/song_end/song_end_C#", mapperName, 0);
	        }
	        
            timeline.LoadAudicaFile(false, path);
            editorInput.SelectMode(EditorMode.Compose);

            genAudicaButton.interactable = true;
            selectSongButton.interactable = false;
        }

        public bool CheckAllUIFilled() {
            if (loadedSong != "" && mapperName != "" && songName != "") {
                return true;
            } else {
                return false;
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
                    UnityEngine.Debug.Log(www.error);
                } else {
                    audioFile = DownloadHandlerAudioClip.GetContent(www);
                    timeline.LoadTimingMode(audioFile);
                    ApplyValues();

                    yield break;
                }
            }
        }

        public AudioClip LoadMp3(string filePath) {
            string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);

            MpegFile mpegFile = new MpegFile(filePath);

            // assign samples into AudioClip
            AudioClip ac = AudioClip.Create(filename,
                (int) (mpegFile.Length / sizeof(float) / mpegFile.Channels),
                mpegFile.Channels,
                mpegFile.SampleRate,
                true,
                data => { int actualReadCount = mpegFile.ReadSamples(data, 0, data.Length); },
                position => { mpegFile = new MpegFile(filePath); });

            //mpegFile.Dispose();

            return ac;
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