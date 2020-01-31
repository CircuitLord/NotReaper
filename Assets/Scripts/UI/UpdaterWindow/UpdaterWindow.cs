using System;
using DG.Tweening;
using NotReaper.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI.UpdaterWindow {
	public class UpdaterWindow : MonoBehaviour {



		public static UpdaterWindow I;

		private AutoUpdaterJSON updateData;


		public Button ignoreButton;
		public Button skipVersionButton;
		public Button updateButton;


		[SerializeField] private TextMeshProUGUI version;
		[SerializeField] private TextMeshProUGUI changelog;


		private void Start() {
			I = this;
			
			Vector3 defaultPos;
			defaultPos.x = 0;
			defaultPos.y = -15f;
			defaultPos.z = -10.0f;
			gameObject.GetComponent<RectTransform>().localPosition = defaultPos;
			gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
			gameObject.SetActive(false);
		}


		public void Activate(AutoUpdaterJSON data) {

			updateData = data;
			
			version.SetText("Current: " + Application.version + " - New: " + data.latestVersion);
			changelog.SetText(data.changelog);
			
			gameObject.SetActive(true);
			
			gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);


		}

		public void Deactivate() {
			gameObject.SetActive(false);
		}



		public void ClickUpdate() {
			ignoreButton.interactable = false;
			skipVersionButton.interactable = false;
			updateButton.interactable = false;
			AutoUpdater.I.StartCoroutine(AutoUpdater.I.DoUpdate(updateData));
		}

		public void ClickIgnore() {
			Deactivate();
		}

		public void ClickSkip() {
			
			PlayerPrefs.SetString("ignoredVersion", updateData.latestVersion);
			Deactivate();
			
			
		}
		
		
		
	}
}