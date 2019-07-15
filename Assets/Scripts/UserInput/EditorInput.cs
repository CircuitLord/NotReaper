using NotReaper.Grid;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Tools;
using NotReaper.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UserInput {


	public enum EditorTool { Standard, Hold, Horizontal, Vertical, ChainStart, ChainNode, Melee, DragSelect, ChainBuilder }


	public class EditorInput : MonoBehaviour {


		public static EditorTool selectedTool = EditorTool.Standard;
		public static TargetHandType selectedHand = TargetHandType.Left;
		public static SnappingMode selectedSnappingMode = SnappingMode.Grid;
		public static bool isOverGrid = false;
		public static bool inUI = false;

		//public PlaceNote toolPlaceNote;
		[SerializeField] public EditorToolkit Tools;
		[SerializeField] private Timeline timeline;

		[SerializeField] private Dropdown soundDropdown;
		[SerializeField] private UINoteHandler UINote;




		public Toggle standardToggle;
		public Toggle holdToggle;
		public Toggle chainStartToggle;
		public Toggle chainNodeToggle;
		public Toggle horzToggle;
		public Toggle vertToggle;
		public Toggle meleeToggle;


		private void Start() {
			InputManager.LoadHotkeys();

			UINote.SelectStandard();
			UINote.SelectLeftHand();
			standardToggle.isOn = true;
		}

		private void Update() {

			if (inUI) return;
			

			if (Input.GetMouseButtonDown(0)) {

				switch (selectedTool) {
					case EditorTool.Standard:
					case EditorTool.Hold:
					case EditorTool.Horizontal:
					case EditorTool.Vertical:
					case EditorTool.ChainStart:
					case EditorTool.ChainNode:
					case EditorTool.Melee:
						Tools.placeNote.TryPlaceNote();
						break;


					default:
						break;
				}


			}

			if (Input.GetMouseButtonDown(1)) {
				switch (selectedTool) {
					case EditorTool.Standard:
					case EditorTool.Hold:
					case EditorTool.Horizontal:
					case EditorTool.Vertical:
					case EditorTool.ChainStart:
					case EditorTool.ChainNode:
					case EditorTool.Melee:
						Tools.placeNote.TryRemoveNote();
						break;


					default:
						break;
				}
			}


			if (Input.GetKeyDown(InputManager.timelineTogglePlay)) {
				timeline.TogglePlayback();
			}



			if (Input.GetKeyDown(InputManager.selectStandard)) {

				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();

				
				selectedTool = EditorTool.Standard;
				standardToggle.isOn = true;

			}
			if (Input.GetKeyDown(InputManager.selectHold)) {
				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();

	
				
				selectedTool = EditorTool.Hold;
				holdToggle.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.selectHorz)) {
				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();

				selectedTool = EditorTool.Horizontal;
				horzToggle.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.selectVert)) {
				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();
				

				selectedTool = EditorTool.Vertical;
				vertToggle.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.selectChainStart)) {
				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();
				

				selectedTool = EditorTool.ChainStart;
				chainStartToggle.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.selectChainNode)) {
				if (selectedTool == EditorTool.Melee) 
					UINote.SelectLeftHand();

				selectedTool = EditorTool.ChainNode;
				chainNodeToggle.isOn = true;
			}
			if (Input.GetKeyDown(InputManager.selectMelee)) {
				selectedTool = EditorTool.Melee;
				meleeToggle.isOn = true;
				UINote.SelectEitherHand();

			}

			if (Input.GetKeyDown(InputManager.toggleColor)) {

				if (selectedHand == TargetHandType.Left) {
					selectedHand = TargetHandType.Right;
					UINote.SelectRightHand();
				}

				else if (selectedHand == TargetHandType.Right) {
					selectedHand = TargetHandType.Left;
					UINote.SelectLeftHand();
				} 
				
				else {
					selectedHand = TargetHandType.Left;
					UINote.SelectLeftHand();
				}

			}

			if (Input.GetKeyDown(InputManager.selectSoundKick)) {
				soundDropdown.value = 0;
			}
			if (Input.GetKeyDown(InputManager.selectSoundSnare)) {
				soundDropdown.value = 1;
			}
			if (Input.GetKeyDown(InputManager.selectSoundPercussion)) {
				soundDropdown.value = 2;
			}
			if (Input.GetKeyDown(InputManager.selectSoundChainStart)) {
				soundDropdown.value = 3;
			}
			if (Input.GetKeyDown(InputManager.selectSoundChainNode)) {
				soundDropdown.value = 4;
			}
			if (Input.GetKeyDown(InputManager.selectSoundMelee)) {
				soundDropdown.value = 5;
			}
		}

	}


}