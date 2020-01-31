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

		public Image nrGuyHyper;
		public Image nrGuy;


		[SerializeField] private TextMeshProUGUI version;
		[SerializeField] private TextMeshProUGUI changelog;

		private Vector3 nrGuyStartPos;

		private void Start() {
			I = this;
			
			Vector3 defaultPos;
			defaultPos.x = 0;
			defaultPos.y = -15f;
			defaultPos.z = -10.0f;
			gameObject.GetComponent<RectTransform>().localPosition = defaultPos;
			gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
			gameObject.SetActive(false);


			nrGuyHyper.DOFade(0f, 0f);
			nrGuy.DOFade(0f, 0f);

			nrGuyStartPos = nrGuy.rectTransform.localPosition;
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

		
		
		private Tweener nrGuyBounce;
		private Tweener nrGuyFade;

		bool isHoveringUpdate = false;
		public void HoverUpdate() {
			if (isHoveringUpdate) return;

			isHoveringUpdate = true;

			
			nrGuy.rectTransform.localPosition = new Vector2(nrGuyStartPos.x, nrGuyStartPos.y - 10f);
			nrGuyBounce = nrGuy.rectTransform.DOLocalMove(nrGuyStartPos, 1f).SetEase(Ease.OutCubic);
			
			nrGuyFade = nrGuy.DOFade(1f, 1f);


		}

		public void LeaveHoverUpdate() {
			nrGuyBounce?.Kill();
			nrGuyFade?.Kill();
			nrGuy.DOFade(0f, 0f);

			isHoveringUpdate = false;
		}
		

		private Tweener nrGuyHyperShake;
		private Tweener nrGuyHyperFade;

		bool IsHoveringSkipVersion = false;
		public void HoverSkipVersion() {
			if (IsHoveringSkipVersion) return;

			IsHoveringSkipVersion = true;

			nrGuyHyperShake = nrGuyHyper.rectTransform.DOShakePosition(100f, 4f, 200);
			
			nrGuyHyperFade = nrGuyHyper.DOFade(1f, 8f);


		}

		public void LeaveHoverSkipVersion() {
			nrGuyHyperShake?.Kill();
			nrGuyHyperFade?.Kill();
			nrGuyHyper.DOFade(0f, 0f);

			IsHoveringSkipVersion = false;
		}
		
		
		
	}
}