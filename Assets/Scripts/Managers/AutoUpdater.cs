using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Michsky.UI.ModernUIPack;
using NotReaper.UI.UpdaterWindow;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Version = System.Version;


namespace NotReaper.Managers {
	public class AutoUpdater : MonoBehaviour {
		public static AutoUpdater I;
		
		
		
		AutoUpdaterJSON updateData = new AutoUpdaterJSON();


		public ProgressBar downloadSlider;
		public TextMeshProUGUI downloadText;


		private void Start() {
			I = this;

			StartCoroutine(Init());

			downloadSlider.currentPercent = 0;
			downloadSlider.gameObject.SetActive(false);


		}







		public IEnumerator DoUpdate(AutoUpdaterJSON data) {
			
			downloadSlider.gameObject.SetActive(true);
			
			if (Application.isEditor) {
				downloadText.SetText("Editor build detected, skipping install process.");
				yield return new WaitForSeconds(2f);
				Application.Quit();
				yield break;
			}
			
			downloadText.SetText("Downloading...");
			
			
			UnityWebRequest www = UnityWebRequest.Get(data.downloadLink);

			string savePath = Path.Combine(Application.dataPath, @"..\", "update.zip");
			www.downloadHandler = new DownloadHandlerFile(savePath);
			
			var operation = www.SendWebRequest();


			while (!operation.isDone) {
				downloadSlider.currentPercent = operation.progress * 100f;
				yield return null;
			}


			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			}
			else {
				Debug.Log("File successfully downloaded and saved to " + savePath);
				downloadSlider.currentPercent = 100;
				downloadText.SetText("Download complete, installing...");

			}



			
			//Launch unzipper
			
			www.downloadHandler.Dispose();
			www.Dispose();
			
			yield return new WaitForSeconds(2f);

			Process.Start(Path.Combine(Application.streamingAssetsPath, "Installer", "NotReaperInstaller.exe"));
			
			Application.Quit();


		}
		

		

		private IEnumerator Init() {
		
			string url = "https://raw.githubusercontent.com/CircuitLord/NotReaper/master/updates.json";
			
			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.SendWebRequest();
 
			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			}
			else {

				updateData = JsonUtility.FromJson<AutoUpdaterJSON>(www.downloadHandler.text);
				
				yield return new WaitForSeconds(5f);
				
				HandleUpdate(updateData);
			}
			
			
		}


		private void HandleUpdate(AutoUpdaterJSON data) {


			if (!IsNewUpdate(data)) return;

			if (IsIgnoringUpdate(data)) return;
			
			
			UpdaterWindow.I.gameObject.SetActive(true);
			UpdaterWindow.I.Activate(data);
			

		}


		private bool IsIgnoringUpdate(AutoUpdaterJSON data) {

			if (data.latestVersion == PlayerPrefs.GetString("ignoredVersion")) return true;

			else return false;
		}
		
		
		private bool IsNewUpdate(AutoUpdaterJSON data) {
			Version newVer = Version.Parse(data.latestVersion);
			
			Version currentVer = Version.Parse(Application.version);
			

			if (newVer.Major > currentVer.Major) return true;

			if (newVer.Minor > currentVer.Minor) return true;

			if (newVer.Build > currentVer.Build) return true;
			
			return false;

		}
		
		
		
		
	}


	[Serializable]
	public class AutoUpdaterJSON {
		public string latestVersion;
		public string downloadLink;
		public string changelog;
		public bool forced;
	}
	
}