using System;
using System.Collections.Generic;
using System.Linq;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Tools {

	public class DragSelect : MonoBehaviour {

		public Timeline timeline;
		public Transform dragSelectTimeline;
		public Transform timelineNotes;
		public GameObject dragSelectGrid;
		public LayerMask notesLayer;
		
		private bool activated = false;
		private bool isDraggingTimeline = false;
		private bool isDraggingGrid = false;
		private bool isDraggingNotesOnGrid = false;
		private bool isDraggingNotesOnTimeline = false;

		// TODO: This should come from whatever new input handler we end up with, for now this at least should abstract input from intent (see CaptureInput())
		// Represent action intents occuring this update frame.
		private bool frameIntentSelect = false;		// click + release
		private bool frameIntentDragStart = false;	// click + drag start frame
		private bool frameIntentDragging = false;	// click + drag frame
		private bool frameIntentDragEnd = false;	// click + drag end frame
		private bool frameIntentCut = false;
		private bool frameIntentCopy = false;
		private bool frameIntentPaste = false;
		private bool frameIntentDelete = false;
		private bool frameIntentDeselectAll = false;
		private bool frameIntentSwapColors = false;
		private bool frameIntentFlipTargetsHorizontally = false;
		private bool frameIntentFlipTargetsVertically = false;
		private bool frameIntentSetHitSoundStandard = false;
		private bool frameIntentSetHitSoundSnare = false;
		private bool frameIntentSetHitSoundPercussion = false;
		private bool frameIntentSetHitSoundChainStart = false;
		private bool frameIntentSetHitSoundChain = false;
		private bool frameIntentSetHitSoundMelee = false;
		
		private bool hasInputDragMovedOutOfClickBounds = false;
		private bool dragStarted = false;
		
		private TargetIcon[] _iconsUnderMouse = new TargetIcon[0];

		private Vector3 startDragMovePos;
		private Vector2 startClickDetectPos;

		private List<Cue> clipboardNotes = new List<Cue>();
		private List<TargetMoveIntent> gridTargetMoveIntents = new List<TargetMoveIntent>();
		private List<TargetMoveIntent> timelineTargetMoveIntents = new List<TargetMoveIntent>();
		private List<TargetSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();

		/** Fetch all icons currently under the mouse
		 *  Will only ever happen once per frame */
		public TargetIcon[] iconsUnderMouse {
			get {
				return _iconsUnderMouse = _iconsUnderMouse == null
					? MouseUtil.IconsUnderMouse(notesLayer)
					: _iconsUnderMouse;
			}
			set { _iconsUnderMouse = value; }
		}

		// Fetch the highest priority target (closest to current time)
		public TargetIcon iconUnderMouse {
			get {
				return iconsUnderMouse != null && iconsUnderMouse.Length > 0
					? iconsUnderMouse[0]
					: null;
			}
		}
		
		//INFO: Code for selecting targets is on the drag select timeline thing itself

		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate(bool active) {
			activated = active;

			if (!active) {
				if (isDraggingTimeline) EndTimelineSelection();
				else if (isDraggingGrid) EndGridSelection();
				timeline.DeselectAllTargets();
			}
		}
		
		public void Update() {
			if (!activated) return;
			CaptureInput();
			UpdateActions();
			UpdateSelections();
			UpdateDragging();
			iconsUnderMouse = null;
		}
		
		public void EndAllDragStuff() {
			if (isDraggingNotesOnTimeline) {
				EndDragTimelineTargetAction();
			} else if (isDraggingNotesOnGrid) {
				EndDragGridTargetAction();
			}

			EndTimelineSelection();
			EndGridSelection();

			foreach (Target target in timeline.selectedNotes) {
				if (target.gridTargetIcon) {
					target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
				}
			}
		}

		private void StartTimelineSelection() {
			timeline.DeselectAllTargets();
			float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
			dragSelectTimeline.SetParent(timelineNotes);
			dragSelectTimeline.position = new Vector3(mouseX, 0, 0);
			dragSelectTimeline.localPosition = new Vector3(dragSelectTimeline.transform.localPosition.x, 0.03f, 0);
			dragSelectTimeline.gameObject.SetActive(true);
			isDraggingTimeline = true;
		}

		private void UpdateTimelineSelection() {
			float diff = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - dragSelectTimeline.position.x;
			float timelineScaleMulti = Timeline.scale / 20f;
			dragSelectTimeline.localScale =
				new Vector3(diff * timelineScaleMulti, 1.1f * (Timeline.scale / 20f), 1);
		}

		private void EndTimelineSelection() {
			isDraggingTimeline = false;
			dragSelectTimeline.gameObject.SetActive(false);
		}


		private void StartGridSelection() {
			timeline.DeselectAllTargets();
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			dragSelectGrid.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
			dragSelectGrid.transform.localScale = new Vector3(0, 0, 1f);
			dragSelectGrid.SetActive(true);
			isDraggingGrid = true;
		}

		private void UpdateGridSelection() {
			Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
			               dragSelectGrid.transform.position;
			dragSelectGrid.transform.localScale = new Vector3(diff.x, diff.y * -1, 1f);
		}

		private void EndGridSelection() {
			dragSelectGrid.SetActive(false);
			isDraggingGrid = false;
		}

		private void StartDragGridTargetAction(TargetIcon icon) {
			isDraggingNotesOnGrid = true;
			startDragMovePos = icon.transform.position;
			gridTargetMoveIntents = new List<TargetMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetMoveIntent();
				var pos = target.gridTargetIcon.transform.localPosition;
				intent.target = target;
				intent.startingPosition = new Vector3(pos.x, pos.y, pos.z);
				gridTargetMoveIntents.Add(intent);
			});
		}

		private void UpdateDragGridTargetAction() {
			var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newPos = NoteGridSnap.SnapToGrid(mousePos, EditorInput.selectedSnappingMode);

			foreach (TargetMoveIntent intent in gridTargetMoveIntents) {

				var offsetFromDragPoint = intent.target.gridTargetPos - startDragMovePos;
				var tempNewPos = newPos + offsetFromDragPoint;
				intent.target.gridTargetIcon.transform.localPosition = new Vector3(tempNewPos.x,
					tempNewPos.y, intent.target.gridTargetPos.z);

				timeline.updateSustainEnd(intent.target);

				intent.intendedPosition =
					new Vector3(tempNewPos.x, tempNewPos.y, intent.target.gridTargetPos.z);
			}
		}

		private void EndDragGridTargetAction() {
			isDraggingNotesOnGrid = false;
			if (gridTargetMoveIntents.Count > 0) {
				timeline.MoveGridTargets(gridTargetMoveIntents);
				gridTargetMoveIntents = new List<TargetMoveIntent>();
			}
		}

		private void StartDragTimelineTargetAction(TargetIcon icon) {
			isDraggingNotesOnTimeline = true;
			startDragMovePos = icon.transform.position;
			timelineTargetMoveIntents = new List<TargetMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetMoveIntent();
				var pos = target.timelineTargetIcon.transform.localPosition;

				intent.target = target;
				intent.startingPosition = new Vector3(pos.x, pos.y, pos.z);

				timelineTargetMoveIntents.Add(intent);
			});
		}

		private void UpdateDragTimelineTargetAction() {
			foreach (TargetMoveIntent intent in timelineTargetMoveIntents) {

				var pos = intent.startingPosition;
				var gridPos = intent.target.gridTargetIcon.transform.localPosition;

				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var offsetFromDragPoint = pos - startDragMovePos;

				Vector3 newPos = SnapToBeat(mousePos);

				newPos += offsetFromDragPoint;
				intent.target.timelineTargetIcon.transform.localPosition =
					new Vector3(newPos.x, pos.y, pos.z);
				intent.target.gridTargetIcon.transform.localPosition =
					new Vector3(gridPos.x, gridPos.y, newPos.x);

				timeline.updateSustainEnd(intent.target);

				intent.intendedPosition = new Vector3(newPos.x, pos.y, pos.z);
			}
		}

		private void EndDragTimelineTargetAction() {

			isDraggingNotesOnTimeline = false;
			if (timelineTargetMoveIntents.Count > 0) {
				timeline.MoveTimelineTargets(timelineTargetMoveIntents);
				timelineTargetMoveIntents = new List<TargetMoveIntent>();
			}
		}

		
		private void TryToggleSelection() {
			if (iconUnderMouse && iconUnderMouse.isSelected) {
				iconUnderMouse.TryDeselect();
			} else if (iconUnderMouse && !iconUnderMouse.isSelected) {
				iconUnderMouse.TrySelect();
			} else {
				timeline.DeselectAllTargets();
			}
		}
		
		private void SetHitsoundAction(TargetVelocity velocity) {
			targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetSetHitsoundIntent();

				intent.target = target;
				intent.startingVelocity = target.velocity;
				intent.newVelocity = velocity;

				targetSetHitsoundIntents.Add(intent);
			});

			timeline.SetTargetHitsounds(targetSetHitsoundIntents);
		}
		
		// Capture raw input and set the state of frame intents 
		private void CaptureInput() {
			// TODO: Move these intents to a new input manager
			bool primaryModifierHeld = false;
			bool secondaryModifierHeld = false;
			
			// all intents are reset every frame
			frameIntentSelect = false;
			frameIntentDragStart = false;
			frameIntentDragging = false;
			frameIntentDragEnd = false;
			frameIntentCut = false;
			frameIntentCopy = false;
			frameIntentPaste = false;
			frameIntentDelete = false;
			frameIntentDeselectAll = false;
			frameIntentSwapColors = false;
			frameIntentFlipTargetsHorizontally = false;
			frameIntentFlipTargetsVertically = false;
			
			// Keyboard input
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) primaryModifierHeld = true;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) secondaryModifierHeld = true;

			if (primaryModifierHeld && !secondaryModifierHeld) {	// permits holding primary and secondary down
				frameIntentFlipTargetsHorizontally = Input.GetKeyDown(KeyCode.F);
				frameIntentCut = Input.GetKeyDown(KeyCode.X);
				frameIntentCopy = Input.GetKeyDown(KeyCode.C);
				frameIntentPaste = Input.GetKeyDown(KeyCode.V);
				frameIntentDeselectAll = Input.GetKeyDown(KeyCode.A);
			}
			else if (secondaryModifierHeld) {
				frameIntentFlipTargetsVertically = Input.GetKeyDown(KeyCode.F);
			}
			else {
				frameIntentDelete = Input.GetKeyDown(KeyCode.Delete);
				frameIntentSwapColors = Input.GetKeyDown(KeyCode.F);
				
				// hitsound selection
				frameIntentSetHitSoundStandard = Input.GetKeyDown(KeyCode.Q);
				frameIntentSetHitSoundSnare = Input.GetKeyDown(KeyCode.W);
				frameIntentSetHitSoundPercussion = Input.GetKeyDown(KeyCode.E);
				frameIntentSetHitSoundChainStart = Input.GetKeyDown(KeyCode.R);
				frameIntentSetHitSoundChain = Input.GetKeyDown(KeyCode.T);
				frameIntentSetHitSoundMelee = Input.GetKeyDown(KeyCode.Y);
			}
			
			// Mouse input
			if (EditorInput.selectedTool == EditorTool.DragSelect) {
				
				if (Input.GetMouseButtonDown(0)) {
					startClickDetectPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				}

				bool isSelectionDown = Input.GetMouseButton(0);

				if (isSelectionDown) {
					frameIntentDragging = hasInputDragMovedOutOfClickBounds;
					if (frameIntentDragging && !dragStarted) {
						frameIntentDragStart = true;
						dragStarted = true;
					}
					else {
						// Check for a tiny amount of mouse movement to test whether a release is meant to be a selection
						float movement = Math.Abs(startClickDetectPos.magnitude - Input.mousePosition.magnitude);
						hasInputDragMovedOutOfClickBounds = (movement > 2);
					}
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				frameIntentDragging = false;
				frameIntentDragEnd = dragStarted;
				frameIntentSelect = !dragStarted;
				
				// these are not reset every frame, only on mouse up!
				hasInputDragMovedOutOfClickBounds = false;
				dragStarted = false;
			}
		}

		// execute simple actions which don't require any state management'
		private void UpdateActions() {
			
			/** Setting hitsounds **/
			if (frameIntentSetHitSoundStandard) SetHitsoundAction(TargetVelocity.Standard);
			if (frameIntentSetHitSoundSnare) SetHitsoundAction(TargetVelocity.Snare);
			if (frameIntentSetHitSoundPercussion) SetHitsoundAction(TargetVelocity.Percussion);
			if (frameIntentSetHitSoundChain) SetHitsoundAction(TargetVelocity.Chain);
			if (frameIntentSetHitSoundChainStart) SetHitsoundAction(TargetVelocity.ChainStart);
			if (frameIntentSetHitSoundMelee) SetHitsoundAction(TargetVelocity.Melee);

			/** Cut Copy Paste Delete **/
			if (frameIntentCut) {
				frameIntentCopy = true;
				frameIntentDelete = true;
			}
			
			if (frameIntentCopy) {
				clipboardNotes = new List<Cue>();
				timeline.selectedNotes.ForEach(note => clipboardNotes.Add(NotePosCalc.ToCue(note, 0, false)));
			}

			if (frameIntentPaste) {
				timeline.DeselectAllTargets();
				timeline.PasteCues(clipboardNotes, Timeline.BeatTime());
			}
			
			if (frameIntentDelete) {
				if (timeline.selectedNotes.Count > 0) {
					timeline.DeleteTargets(timeline.selectedNotes);
				}
				timeline.selectedNotes = new List<Target>();
			}
			
			/** Note flipping **/
			if (frameIntentSwapColors) timeline.SwapTargets(timeline.selectedNotes);
			if (frameIntentFlipTargetsHorizontally) timeline.FlipTargetsHorizontal(timeline.selectedNotes);
			if (frameIntentFlipTargetsVertically) timeline.FlipTargetsVertical(timeline.selectedNotes);

			/** Note selection and movement **/
			if (frameIntentDeselectAll) timeline.DeselectAllTargets();
		}

		// Update the selection box and individually added / removed targets
		private void UpdateSelections() {
			
			/** Clicking to add or remove targets **/
			if (frameIntentSelect) TryToggleSelection();

			/** Selection outline **/
			if (frameIntentDragStart) {
				// if we're not hovering over an icon, start a new outline
				if (!iconUnderMouse) {
					if (timeline.hover) StartTimelineSelection();
					else StartGridSelection();
				}
			}

			if (frameIntentDragging) {
				if (isDraggingTimeline) UpdateTimelineSelection();
				else if (isDraggingGrid) UpdateGridSelection();
			}

			if (frameIntentDragEnd) {
				if (isDraggingGrid || isDraggingTimeline) EndAllDragStuff();
			}
		}

		// Update the drag-around-the-area logic of selected targets
		private void UpdateDragging() {
			bool shouldDragNotes = (timeline.selectedNotes.Count > 0) || iconUnderMouse;
			if (shouldDragNotes) {
				/** Moving targets around the timeline or grid **/
				if (frameIntentDragStart) {

					bool anySelectedIconUnderMouse = iconsUnderMouse.Where(icon => icon.isSelected).ToArray().Length != 0;
					if (iconUnderMouse && !anySelectedIconUnderMouse) {
						TryToggleSelection();
						anySelectedIconUnderMouse = true;
					}
					
					if (anySelectedIconUnderMouse) {
						if (iconUnderMouse.location == TargetIconLocation.Grid) StartDragGridTargetAction(iconUnderMouse);
						if (iconUnderMouse.location == TargetIconLocation.Timeline) StartDragTimelineTargetAction(iconUnderMouse);
					}
				}

				if (frameIntentDragging) {
					if (isDraggingNotesOnGrid) UpdateDragGridTargetAction();
					if (isDraggingNotesOnTimeline) UpdateDragTimelineTargetAction();
				}
			}

			if (frameIntentDragEnd) {
				if (isDraggingNotesOnGrid) EndDragGridTargetAction();
				if (isDraggingNotesOnTimeline) EndDragTimelineTargetAction();

				foreach (Target target in timeline.selectedNotes) {
					if (target.gridTargetIcon) {
						target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
					}
				}
			}
		}
		
		private Vector3 SnapToBeat(Vector3 position) {
			var increments = ((480 / timeline.beatSnap) * 4f) / 480; // what even is life //42
			return new Vector3(
				Mathf.Round(position.x / increments) * increments,
				position.y,
				position.z
			);
		}
	}
}