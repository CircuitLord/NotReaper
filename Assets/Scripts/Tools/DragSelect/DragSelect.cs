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
		bool isSelectionDown = false;
		bool hasMovedOutOfClickBounds = false;

		Vector2 startGridMovePos;
		float startTimelineMoveTime;

		Vector2 startClickDetectPos;

		public Transform dragSelectTimeline;
		public Transform timelineNotes;

		public GameObject dragSelectGrid;

		public LayerMask notesLayer;

		public List<Cue> clipboardNotes = new List<Cue>();

		private List<TargetGridMoveIntent> gridTargetMoveIntents = new List<TargetGridMoveIntent>();
		private List<TargetTimelineMoveIntent> timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();

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
			startGridMovePos = icon.data.position;

			gridTargetMoveIntents = new List<TargetGridMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetGridMoveIntent();

				intent.target = target.data;
				intent.startingPosition = new Vector2(target.data.x, target.data.y);
				
				gridTargetMoveIntents.Add(intent);
			});
		}

		public void EndDragGridTargetAction() {

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

		public void EndDragTimelineTargetAction() {

			isDraggingNotesOnTimeline = false;
			if (timelineTargetMoveIntents.Count > 0) {
				timeline.MoveTimelineTargets(timelineTargetMoveIntents);
				timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
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


		private void StartSelectionAction() {
			isSelectionDown = true;
			hasMovedOutOfClickBounds = false;
			startClickDetectPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}

		public void EndSelectionAction() {
			isSelectionDown = false;
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


		public void EndAllDragStuff() {
			if (isDraggingNotesOnTimeline) {
				EndDragTimelineTargetAction();
			} else if (isDraggingNotesOnGrid) {
				EndDragGridTargetAction();
			}

			EndTimelineDrag();
			EndGridDrag();
		}


		void Update() {

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
			if (isSelectionDown && !hasMovedOutOfClickBounds) {

				// Check for a tiny amount of mouse movement to ensure this was meant to be a click

				float movement = Math.Abs(startClickDetectPos.magnitude - Input.mousePosition.magnitude);
				if (movement > 2) {
					hasMovedOutOfClickBounds = true;
				}
			}

			/** Cut Copy Paste Delete **/
			// TODO: Move these actions into timeline to record sane undo actions!
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

					} else if (iconUnderMouse && !isSelectionDown) {
						StartSelectionAction();
					} else if (iconUnderMouse && timeline.selectedNotes.Count == 0 && hasMovedOutOfClickBounds) {
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
					if (!hasMovedOutOfClickBounds) TryToggleSelection();
				}

				if (isDraggingNotesOnGrid) {

					// TODO: this should really be handled by intermediary semi-transparent objects rather than updating "real" state as we go ...

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

				if (isDraggingNotesOnTimeline) {
					foreach (TargetTimelineMoveIntent intent in timelineTargetMoveIntents) {
						float offsetFromDragPoint = intent.startTime - startTimelineMoveTime;
						var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						float newTime = SnapToBeat(mousePos);
						newTime += offsetFromDragPoint;

						intent.target.beatTime = newTime;
						intent.intendedTime = newTime;
					}
				}
			}
		}

		private float SnapToBeat(Vector3 position) {
			var increments = ((480 / timeline.beatSnap) * 4f) / 480;
			return Timeline.DurationToBeats(Timeline.time) + Mathf.Round(position.x / increments) * increments;
		}
	}
}