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

		private Vector2 startGridMovePos;
		private Vector2 startClickDetectPos;
		private float startTimelineMoveTime;

		private List<Cue> clipboardNotes = new List<Cue>();
		private List<TargetGridMoveIntent> gridTargetMoveIntents = new List<TargetGridMoveIntent>();
		private List<TargetTimelineMoveIntent> timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
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
			startGridMovePos = icon.data.position;
			
			gridTargetMoveIntents = new List<TargetGridMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetGridMoveIntent();
				intent.target = target.data;
				intent.startingPosition = new Vector2(target.data.x, target.data.y);
				
				gridTargetMoveIntents.Add(intent);
			});
		}

		private void UpdateDragGridTargetAction() {
			var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var newPosVec3 = NoteGridSnap.SnapToGrid(mousePos, EditorInput.selectedSnappingMode);
			Vector2 newPos = new Vector2(newPosVec3.x, newPosVec3.y);

			foreach (TargetGridMoveIntent intent in gridTargetMoveIntents) {

				var offsetFromDragPoint = intent.startingPosition - startGridMovePos;
				var tempNewPos = newPos + offsetFromDragPoint;
				intent.target.position = tempNewPos;

				intent.intendedPosition = tempNewPos;
			}
		}

		private void EndDragGridTargetAction() {
			isDraggingNotesOnGrid = false;
			if (gridTargetMoveIntents.Count > 0) {
				timeline.MoveGridTargets(gridTargetMoveIntents);
				gridTargetMoveIntents = new List<TargetGridMoveIntent>();
			}
		}

		private void StartDragTimelineTargetAction(TargetIcon icon) {
			isDraggingNotesOnTimeline = true;
			startTimelineMoveTime = icon.data.beatTime;
			timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetTimelineMoveIntent();
				intent.target = target.data;
				intent.startTime = target.data.beatTime;

				timelineTargetMoveIntents.Add(intent);
			});
		}

		private void UpdateDragTimelineTargetAction() {
			foreach (TargetTimelineMoveIntent intent in timelineTargetMoveIntents) {

				float offsetFromDragPoint = intent.startTime - startTimelineMoveTime;
				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePos.x /= Timeline.scaleTransform;
				float newTime = SnapToBeat(mousePos);

				newTime += offsetFromDragPoint;
				intent.target.beatTime = newTime;

				intent.intendedTime = newTime;
			}
		}

		private void EndDragTimelineTargetAction() {

			isDraggingNotesOnTimeline = false;
			if (timelineTargetMoveIntents.Count > 0) {
				timeline.MoveTimelineTargets(timelineTargetMoveIntents);
				timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
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

				intent.target = target.data;
				intent.startingVelocity = target.data.velocity;
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
				frameIntentDeselectAll = Input.GetKeyDown(KeyCode.D);
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
			}
		}
		
		private float SnapToBeat(Vector3 position) {
			var increments = ((480 / timeline.beatSnap) * 4f) / 480;
			return Timeline.DurationToBeats(Timeline.time) + Mathf.Round(position.x / increments) * increments; 
		}
	}
}