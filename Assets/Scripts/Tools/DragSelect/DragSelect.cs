using System;
using System.Collections;
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

		public bool activated = false;
		public bool isDraggingTimeline = false;
		public bool isDraggingGrid = false;
		public bool isDraggingNotesOnGrid = false;
		public bool isDraggingNotesOnTimeline = false;
		bool isSelectionDownOld = false;
		bool hasMovedOutOfClickBoundsOld = false;

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
		
		bool hasInputDragMovedOutOfClickBounds = false;
		
		Vector3 startDragMovePos;
		Vector2 startClickDetectPos;

		public Transform dragSelectTimeline;
		public Transform timelineNotes;

		public GameObject dragSelectGrid;

		public LayerMask notesLayer;

		public List<Cue> clipboardNotes = new List<Cue>();

		private List<TargetMoveIntent> gridTargetMoveIntents = new List<TargetMoveIntent>();
		private List<TargetMoveIntent> timelineTargetMoveIntents = new List<TargetMoveIntent>();

		private List<TargetSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();

		//INFO: Code for selecting targets is on the drag select timeline thing itself

		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate(bool active) {
			activated = active;

			if (!active) {
				if (isDraggingTimeline) EndTimelineDrag();
				else if (isDraggingGrid) EndGridDrag();
				timeline.DeselectAllTargets();
			}


		}


		private void StartTimelineDrag() {

			timeline.DeselectAllTargets();

			float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

			dragSelectTimeline.SetParent(timelineNotes);
			dragSelectTimeline.position = new Vector3(mouseX, 0, 0);
			dragSelectTimeline.localPosition = new Vector3(dragSelectTimeline.transform.localPosition.x, 0.03f, 0);

			dragSelectTimeline.gameObject.SetActive(true);

			isDraggingTimeline = true;
		}

		public void EndTimelineDrag() {
			isDraggingTimeline = false;

			dragSelectTimeline.gameObject.SetActive(false);

		}


		private void StartGridDrag() {

			timeline.DeselectAllTargets();

			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			dragSelectGrid.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
			dragSelectGrid.transform.localScale = new Vector3(0, 0, 1f);

			dragSelectGrid.SetActive(true);


			isDraggingGrid = true;
		}

		public void EndGridDrag() {

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

		public void EndDragGridTargetAction() {

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

		public void EndDragTimelineTargetAction() {

			isDraggingNotesOnTimeline = false;
			if (timelineTargetMoveIntents.Count > 0) {
				timeline.MoveTimelineTargets(timelineTargetMoveIntents);
				timelineTargetMoveIntents = new List<TargetMoveIntent>();
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


		private void StartSelectionAction() {
			isSelectionDownOld = true;
			hasMovedOutOfClickBoundsOld = false;
			startClickDetectPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}

		public void EndSelectionAction() {
			isSelectionDownOld = false;
		}

		private void TryToggleSelection() {
			var iconsUnderMouse = MouseUtil.IconsUnderMouse(notesLayer);
			TargetIcon iconUnderMouse = iconsUnderMouse.Length > 0 ? iconsUnderMouse[0] : null;

			if (iconUnderMouse && iconUnderMouse.isSelected) {
				iconUnderMouse.TryDeselect();
			} else if (iconUnderMouse && !iconUnderMouse.isSelected) {
				iconUnderMouse.TrySelect();
			}
		}


		public Vector3 CalcAvgNotePos(List<Target> targets) {

			Vector3 avgPos = new Vector3(0, 0, 0);

			foreach (Target target in targets) {
				avgPos += target.gridTargetPos;

			}
			return avgPos / targets.Count;
		}


		public void EndAllDragStuff() {
			if (isDraggingNotesOnTimeline) {
				EndDragTimelineTargetAction();
			} else if (isDraggingNotesOnGrid) {
				EndDragGridTargetAction();
			}

			EndTimelineDrag();
			EndGridDrag();

			foreach (Target target in timeline.selectedNotes) {
				if (target.gridTargetIcon) {
					target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
				}
			}

			//EndSelectionAction();
		}

		void CaptureInput() {
			bool primaryModifierHeld = false;
			bool secondaryModifierHeld = false;
			frameIntentSelect = false;
			frameIntentDragStart = false;
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
			
			if (Input.GetMouseButtonDown(0)) {
				startClickDetectPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			
			bool isSelectionDown = Input.GetMouseButton(0);

			if (isSelectionDown) {
				if (hasInputDragMovedOutOfClickBounds && !frameIntentDragging) {
					frameIntentDragStart = true;
					frameIntentDragging = true;
				} else {
					// Check for a tiny amount of mouse movement to test whether a release is meant to be a selection
					float movement = Math.Abs(startClickDetectPos.magnitude - Input.mousePosition.magnitude);
					hasInputDragMovedOutOfClickBounds = (movement > 2);
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				frameIntentDragEnd = hasInputDragMovedOutOfClickBounds;
				frameIntentSelect = !hasInputDragMovedOutOfClickBounds;
				
				hasInputDragMovedOutOfClickBounds = false; 
				frameIntentDragging = false;
			}
		}

		void Update() {

			CaptureInput();
			if (frameIntentDragStart) Debug.Log("drag start");
			if (frameIntentDragging) Debug.Log("drag");
			if (frameIntentDragEnd) Debug.Log("drag end");
			if (frameIntentSelect) Debug.Log("click");

			//If the user decides they hate productivity and want to unselect all their notes, so be it.
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
				timeline.DeselectAllTargets();
			}

			//Applying hitsounds to selected:

			if (!activated) return;


			//TODO: Add shift + ctrl detection up here instead of multipule times


			if (Input.GetKeyDown(KeyCode.Q)) {
				SetHitsoundAction(TargetVelocity.Standard);
			} else if (Input.GetKeyDown(KeyCode.W)) {
				SetHitsoundAction(TargetVelocity.Snare);
			} else if (Input.GetKeyDown(KeyCode.E)) {
				SetHitsoundAction(TargetVelocity.Percussion);
			} else if (Input.GetKeyDown(KeyCode.R)) {
				SetHitsoundAction(TargetVelocity.ChainStart);
			} else if (Input.GetKeyDown(KeyCode.T)) {
				SetHitsoundAction(TargetVelocity.Chain);
			} else if (Input.GetKeyDown(KeyCode.Y)) {
				SetHitsoundAction(TargetVelocity.Melee);
			}


			var iconsUnderMouse = MouseUtil.IconsUnderMouse(notesLayer);
			TargetIcon iconUnderMouse = iconsUnderMouse.Length > 0 ? iconsUnderMouse[0] : null;

			/** Click Detection **/
			if (isSelectionDownOld && !hasMovedOutOfClickBoundsOld) {

				// Check for a tiny amount of mouse movement to ensure this was meant to be a click

				float movement = Math.Abs(startClickDetectPos.magnitude - Input.mousePosition.magnitude);
				if (movement > 2) {
					hasMovedOutOfClickBoundsOld = true;
				}
			}

			/** Cut Copy Paste Delete **/
			Action delete = () => {
				if (timeline.selectedNotes.Count > 0) {
					timeline.DeleteTargets(timeline.selectedNotes);
				}
				timeline.selectedNotes = new List<Target>();
			};

			Action copy = () => {
				clipboardNotes = new List<Cue>();
				timeline.selectedNotes.ForEach(note => clipboardNotes.Add(NotePosCalc.ToCue(note, 0, false)));
			};

			Action paste = () => {
				timeline.DeselectAllTargets();
				timeline.PasteCues(clipboardNotes, Timeline.BeatTime());
			};

			if (Input.GetKeyDown(KeyCode.Delete)) {
				delete();
			}

			bool dev = false;
			bool modifierHeld = dev ?
				Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) :
				Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

			if (modifierHeld) {
				if (Input.GetKeyDown(KeyCode.X)) {
					copy();
					delete();
				}

				if (Input.GetKeyDown(KeyCode.C)) {
					copy();
				}

				if (Input.GetKeyDown(KeyCode.V)) {
					paste();
				}
			}

			/** Note flipping **/
			if (Input.GetKeyDown(KeyCode.F)) {

				var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

				// flip horizontal
				if (ctrlHeld && !shiftHeld) {
					timeline.FlipTargetsHorizontal(timeline.selectedNotes);
				}

				// flip vertical
				else if (shiftHeld) {
					timeline.FlipTargetsVertical(timeline.selectedNotes);
				}

				// invert
				else {
					timeline.SwapTargets(timeline.selectedNotes);
				}
			}

			/** Click + Drag Handling **/
			//TODOUNDO I think I fixed this? //TODO: it should deselect when resiszing the grid dragger, but not deselect when scrubbing through the timeline while grid dragging

			if (EditorInput.selectedTool == EditorTool.DragSelect) {

				if (Input.GetMouseButtonDown(0)) {

					if (iconUnderMouse) {
						var anySelectedIconUnderMouse = iconsUnderMouse.Where(icon => icon.isSelected).ToArray();
						if (anySelectedIconUnderMouse.Length == 0) return;
						switch (iconUnderMouse.location) {
							case TargetIconLocation.Grid:
								StartDragGridTargetAction(iconUnderMouse);
								break;
							case TargetIconLocation.Timeline:
								StartDragTimelineTargetAction(iconUnderMouse);
								break;
						}
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					if (isDraggingNotesOnGrid) EndDragGridTargetAction();
					if (isDraggingNotesOnTimeline) EndDragTimelineTargetAction();

					foreach (Target target in timeline.selectedNotes) {
						if (target.gridTargetIcon) {
							target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
						}
					}
				}

				if (Input.GetMouseButton(0)) {

					//If we're not already dragging

					if (!isDraggingTimeline &&
						!isDraggingGrid &&
						!isDraggingNotesOnGrid &&
						!isDraggingNotesOnTimeline &&
						!iconUnderMouse
					) {
						if (timeline.hover) {
							StartTimelineDrag();
						} else {
							StartGridDrag();
						}
					} else if (isDraggingTimeline) {
						float diff = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - dragSelectTimeline.position.x;
						float timelineScaleMulti = Timeline.scale / 20f;
						dragSelectTimeline.localScale = new Vector3(diff * timelineScaleMulti, 1.1f * (Timeline.scale / 20f), 1);
					} else if (isDraggingGrid) {

						Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragSelectGrid.transform.position;
						dragSelectGrid.transform.localScale = new Vector3(diff.x, diff.y * -1, 1f);

					} else if (iconUnderMouse && !isSelectionDownOld) {
						StartSelectionAction();
					} else if (iconUnderMouse && timeline.selectedNotes.Count == 0 && hasMovedOutOfClickBoundsOld) {
						iconUnderMouse.TrySelect();
						if (iconUnderMouse.location == TargetIconLocation.Grid) StartDragGridTargetAction(iconUnderMouse);
						if (iconUnderMouse.location == TargetIconLocation.Timeline) StartDragTimelineTargetAction(iconUnderMouse);
					}
				} else {
					if (isDraggingTimeline) EndTimelineDrag();
					else if (isDraggingGrid) EndGridDrag();
				}
				if (Input.GetMouseButtonUp(0)) {
					EndSelectionAction();
					if (!hasMovedOutOfClickBoundsOld) TryToggleSelection();
				}

				if (isDraggingNotesOnGrid) {

					// TODO: this should really be handled by intermediary semi-transparent objects rather than updating "real" state as we go ...

					var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					Vector3 newPos = NoteGridSnap.SnapToGrid(mousePos, EditorInput.selectedSnappingMode);

					foreach (TargetMoveIntent intent in gridTargetMoveIntents) {

						var offsetFromDragPoint = intent.target.gridTargetPos - startDragMovePos;
						var tempNewPos = newPos + offsetFromDragPoint;
						intent.target.gridTargetIcon.transform.localPosition = new Vector3(tempNewPos.x, tempNewPos.y, intent.target.gridTargetPos.z);
						
						timeline.updateSustainEnd(intent.target);

						intent.intendedPosition = new Vector3(tempNewPos.x, tempNewPos.y, intent.target.gridTargetPos.z);
					}
				}

				if (isDraggingNotesOnTimeline) {
					foreach (TargetMoveIntent intent in timelineTargetMoveIntents) {

						var pos = intent.startingPosition;
						var gridPos = intent.target.gridTargetIcon.transform.localPosition;

						var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						var offsetFromDragPoint = pos - startDragMovePos;

						Vector3 newPos = SnapToBeat(mousePos);
						// TODO: Snap!

						newPos += offsetFromDragPoint;
						intent.target.timelineTargetIcon.transform.localPosition = new Vector3(newPos.x, pos.y, pos.z);
						intent.target.gridTargetIcon.transform.localPosition = new Vector3(gridPos.x, gridPos.y, newPos.x);

						timeline.updateSustainEnd(intent.target);

						intent.intendedPosition = new Vector3(newPos.x, pos.y, pos.z);
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