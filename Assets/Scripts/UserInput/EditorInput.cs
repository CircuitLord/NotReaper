using UnityEngine;

namespace NotReaper.UserInput {


	public class EditorInput : MonoBehaviour {

		public enum EditorTool { Standard, Hold, Horizontal, Vertical, ChainStart, ChainNode, Melee, DragSelect, ChainBuilder }
		public enum EditorSnappingMode { Grid, Melee, None }

		public EditorTool selectedTool;

		private EditorSnappingMode snappingMode;


		private void Start() {
			selectedTool = EditorTool.Standard;
		}

		//I'm using smoot
		private void Update() {


			if (Input.GetMouseButtonDown(0)) {

				switch (selectedTool) {
					case EditorTool.Standard:
					case EditorTool.Hold:
					case EditorTool.Horizontal:
					case EditorTool.Vertical:
					case EditorTool.ChainStart:
					case EditorTool.ChainNode:
					case EditorTool.Melee:


					default:
						return;
				}


			}


			if (Input.GetKeyDown(InputManager.IM.selectStandard)) {

			}
			if (Input.GetKeyDown(InputManager.IM.selectHold)) {
				//holdButton.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.IM.selectHorz)) {
				//horzButton.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.IM.selectVert)) {
				//vertButton.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.IM.selectChainStart)) {
				//chainStartButton.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.IM.selectChainNode)) {
				//chainPartButton.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.IM.selectMelee)) {
				//meleeButton.isOn = true;
			}

			if (Input.GetKeyDown(InputManager.IM.toggleColor)) {
				/*
								if (L.isOn == false) {
									L.isOn = true;
								} else if (R.isOn == false) {
									R.isOn = true;
								} else {
									L.isOn = true;
								}
								*/

			}

			if (Input.GetKeyDown(InputManager.IM.selectSoundKick)) {
				//soundSelect.value = 0;
			}
			if (Input.GetKeyDown(InputManager.IM.selectSoundSnare)) {
				//soundSelect.value = 1;
			}
			if (Input.GetKeyDown(InputManager.IM.selectSoundPercussion)) {
				//soundSelect.value = 2;
			}
			if (Input.GetKeyDown(InputManager.IM.selectSoundChainStart)) {
				//soundSelect.value = 3;
			}
			if (Input.GetKeyDown(InputManager.IM.selectSoundChainNode)) {
				//soundSelect.value = 4;
			}
			if (Input.GetKeyDown(InputManager.IM.selectSoundMelee)) {
				//soundSelect.value = 5;
			}
		}

	}


}