using NotReaper.Grid;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.Tools;
using NotReaper.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NotReaper.UserInput {


	public enum EditorTool { Standard, Hold, Horizontal, Vertical, ChainStart, ChainNode, Melee, DragSelect, ChainBuilder }


	public class EditorInput : MonoBehaviour {


		public static EditorTool selectedTool = EditorTool.Standard;
		public static TargetHandType selectedHand = TargetHandType.Left;
		public static SnappingMode selectedSnappingMode = SnappingMode.Grid;
		public static TargetBehavior selectedBehavior = TargetBehavior.Standard;
		public static DropdownToVelocity selectedVelocity = DropdownToVelocity.Standard;
		public static bool isOverGrid = false;
		public static bool inUI = false;

		//public PlaceNote toolPlaceNote;
		[SerializeField] public EditorToolkit Tools;
		[SerializeField] private Timeline timeline;

		[SerializeField] private Dropdown soundDropdown;

		public Toggle standardToggle;
		public Toggle holdToggle;
		public Toggle chainStartToggle;
		public Toggle chainNodeToggle;
		public Toggle horzToggle;
		public Toggle vertToggle;
		public Toggle meleeToggle;

		public TargetIcon hover;

        public NoteGridSnap noteGrid;

		bool isCTRLDown;
		bool isShiftDown;

		private void Start() {
			InputManager.LoadHotkeys();

			SelectTool(EditorTool.Standard);
			SelectHand(TargetHandType.Left);


		}

		public void SelectHand(TargetHandType type) {
			selectedHand = type;
			hover.SetHandType(type);
		}
		
		/// <summary>
		/// Select a sound for the current tool.
		/// </summary>
		/// <param name="velocity">The "sound" type to play.</param>
		public void SelectVelocity(DropdownToVelocity velocity) {
			
			switch (velocity) {
				case DropdownToVelocity.Standard:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.Standard);
					break;
				case DropdownToVelocity.Snare:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.Snare);
					break;

				case DropdownToVelocity.Percussion:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.Percussion);
					break;

				case DropdownToVelocity.ChainStart:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.ChainStart);
					break;

				case DropdownToVelocity.Chain:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.Chain);
					break;

				case DropdownToVelocity.Melee:
					soundDropdown.SetValueWithoutNotify((int)DropdownToVelocity.Melee);
					break;
				
				
			}

			selectedVelocity = velocity;
			
		}

		public void SelectTool(EditorTool tool) {

            

			//Update the UI based on the tool:
			switch (tool) {
				case EditorTool.Standard:
					selectedBehavior = TargetBehavior.Standard;
					standardToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Standard);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Standard);
					noteGrid.SetSnappingMode(SnappingMode.Grid);
					break;

				case EditorTool.Hold:
					selectedBehavior = TargetBehavior.Hold;
					holdToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Hold);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Standard);
					noteGrid.SetSnappingMode(SnappingMode.Grid);
					break;

				case EditorTool.Horizontal:
					selectedBehavior = TargetBehavior.Horizontal;
					horzToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Horizontal);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Standard);
					noteGrid.SetSnappingMode(SnappingMode.Grid);
					break;

				case EditorTool.Vertical:
					selectedBehavior = TargetBehavior.Vertical;
					vertToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Vertical);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Standard);
					noteGrid.SetSnappingMode(SnappingMode.Grid);
					break;

				case EditorTool.ChainStart:
					selectedBehavior = TargetBehavior.ChainStart;
					chainStartToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.ChainStart);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.ChainStart);
					noteGrid.SetSnappingMode(SnappingMode.Grid);
					break;

				case EditorTool.ChainNode:
					selectedBehavior = TargetBehavior.Chain;
					chainNodeToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Chain);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Chain);
					noteGrid.SetSnappingMode(SnappingMode.None);
					break;

				case EditorTool.Melee:
					selectedBehavior = TargetBehavior.Melee;
					meleeToggle.SetIsOnWithoutNotify(true);
					hover.SetBehavior(TargetBehavior.Melee);
					soundDropdown.SetValueWithoutNotify((int) DropdownToVelocity.Melee);
					noteGrid.SetSnappingMode(SnappingMode.Melee);
					SelectHand(TargetHandType.Either);
					break;

				default:
					break;
			}

			selectedTool = tool;

		}


		private void Update() {

			if (inUI) return;

			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
				isCTRLDown = true;
			} else {
				isCTRLDown = true;
			}

			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				isShiftDown = true;
			} else {
				isShiftDown = false;
			}



			

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

				SelectTool(EditorTool.Standard);

			}
			if (Input.GetKeyDown(InputManager.selectHold)) {
				SelectTool(EditorTool.Hold);
			}
			if (Input.GetKeyDown(InputManager.selectHorz)) {
				SelectTool(EditorTool.Horizontal);
			}
			if (Input.GetKeyDown(InputManager.selectVert)) {
				SelectTool(EditorTool.Vertical);
			}
			if (Input.GetKeyDown(InputManager.selectChainStart)) {
				SelectTool(EditorTool.ChainStart);
			}
			if (Input.GetKeyDown(InputManager.selectChainNode)) {
				SelectTool(EditorTool.ChainNode);
			}
			if (Input.GetKeyDown(InputManager.selectMelee)) {
				SelectTool(EditorTool.Melee);

			}

			if (Input.GetKeyDown(InputManager.toggleColor)) {

				if (selectedHand == TargetHandType.Left) {
					SelectHand(TargetHandType.Right);
				}

				else if (selectedHand == TargetHandType.Right) {
					SelectHand(TargetHandType.Left);
				} 
				
				else {
					SelectHand(TargetHandType.Left);
				}

			}

			if (Input.GetKeyDown(InputManager.selectSoundKick)) {
				soundDropdown.value = 0;
			}
			else if (Input.GetKeyDown(InputManager.selectSoundSnare)) {
				soundDropdown.value = 1;
			}
			else if (Input.GetKeyDown(InputManager.selectSoundPercussion)) {
				soundDropdown.value = 2;
			}
			else if (Input.GetKeyDown(InputManager.selectSoundChainStart)) {
				soundDropdown.value = 3;
			}
			else if (Input.GetKeyDown(InputManager.selectSoundChainNode)) {
				soundDropdown.value = 4;
			}
			else if (Input.GetKeyDown(InputManager.selectSoundMelee)) {
				soundDropdown.value = 5;
			}


			if (!isShiftDown && isCTRLDown && Input.GetKeyDown(InputManager.undo)) {
				Tools.undoRedoManager.Undo();
				Debug.Log("Undoing...");
			}

			if (isShiftDown && isCTRLDown && Input.GetKeyDown(InputManager.redo)) {
				Tools.undoRedoManager.Redo();
				Debug.Log("Redoing...");
			}

		}

	}


}