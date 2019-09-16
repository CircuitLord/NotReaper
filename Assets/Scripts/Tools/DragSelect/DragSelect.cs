using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Tools {

	public class DragSelect : MonoBehaviour {

		public Timeline timeline;

		public bool activated = false;
		bool isDraggingTimeline = false;
		bool isDraggingGrid = false;
		bool isDraggingNotesOnGrid = false;
		bool isDraggingNotesOnTimeline = false;
		bool isSelectionDown = false;
		bool hasMovedOutOfClickBounds = false;

		Vector3 startDragMovePos;
		Vector2 startClickDetectPos;

		public Transform dragSelectTimeline;
		public Transform timelineNotes;

		public GameObject dragSelectGrid;

		public LayerMask notesLayer;

		public List<Cue> clipboardNotes = new List<Cue>();

		private List<TargetMoveIntent> gridTargetMoveIntents = new List<TargetMoveIntent>();
		private List<TargetMoveIntent> timelineTargetMoveIntents = new List<TargetMoveIntent>();

		//INFO: Code for selecting targets is on the drag select timline thing itself

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

		private void EndTimelineDrag() {
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

		private void EndGridDrag() {

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

		private void EndDragTimelineTargetAction() {

			isDraggingNotesOnTimeline = false;
			if (timelineTargetMoveIntents.Count > 0) {
				timeline.MoveTimelineTargets(timelineTargetMoveIntents);
				timelineTargetMoveIntents = new List<TargetMoveIntent>();
			}
		}

		private void StartSelectionAction() {
			isSelectionDown = true;
			hasMovedOutOfClickBounds = false;
			startClickDetectPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}

		private void EndSelectionAction() {
			isSelectionDown = false;
		}

		private void TryToggleSelection() {
			var iconsUnderMouse = MouseUtil.IconsUnderMouse(notesLayer);
			TargetIcon iconUnderMouse = iconsUnderMouse.Length > 0 ? iconsUnderMouse[0] : null;

			if (iconUnderMouse && iconUnderMouse.isSelected) {
				iconUnderMouse.TryDeselect();
			}
			else if (iconUnderMouse && !iconUnderMouse.isSelected) {
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


		void Update() {

			//If the user decides they hate productivity and want to unselect all their notes, so be it.
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
				timeline.DeselectAllTargets();
			} 

			if (!activated) return;

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
					timeline.DeleteTargets(timeline.selectedNotes, true, true);
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
						if (iconUnderMouse.location == TargetIconLocation.Grid) StartDragGridTargetAction(iconUnderMouse);
						if (iconUnderMouse.location == TargetIconLocation.Timeline) StartDragTimelineTargetAction(iconUnderMouse);
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					if (isDraggingNotesOnGrid) EndDragGridTargetAction();
					if (isDraggingNotesOnTimeline) EndDragTimelineTargetAction();

					foreach (Target target in timeline.selectedNotes) {

						//TODO: does changing target in one list change it in another? no i don't think so

						target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
					}
				}

				if (Input.GetMouseButton(0)) {

					//If we're not already dragging

					if (
						!isDraggingTimeline &&
						!isDraggingGrid &&
						!isDraggingNotesOnGrid &&
						!isDraggingNotesOnTimeline &&
						!iconUnderMouse
					) {
						if (timeline.hover) {
							StartTimelineDrag();
						}
						else {
							StartGridDrag();
						}
					}
					else if (isDraggingTimeline) {
						float diff = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - dragSelectTimeline.position.x;
						float timelineScaleMulti = Timeline.scale / 20f;
						dragSelectTimeline.localScale = new Vector3(diff * timelineScaleMulti, 1.1f * (Timeline.scale / 20f), 1);
					}
					else if (isDraggingGrid) {

						Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragSelectGrid.transform.position;
						dragSelectGrid.transform.localScale = new Vector3(diff.x, diff.y * -1, 1f);

					}
					else if (iconUnderMouse && !isSelectionDown) {
						StartSelectionAction();
					}
					else if (iconUnderMouse && timeline.selectedNotes.Count == 0 && hasMovedOutOfClickBounds) {
						iconUnderMouse.TrySelect();
						if (iconUnderMouse.location == TargetIconLocation.Grid) StartDragGridTargetAction(iconUnderMouse);
						if (iconUnderMouse.location == TargetIconLocation.Timeline) StartDragTimelineTargetAction(iconUnderMouse);
					}
				}
				else {
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
					Vector3 newPos = NoteGridSnap.SnapToGrid(mousePos, EditorInput.selectedSnappingMode);

					foreach (TargetMoveIntent intent in gridTargetMoveIntents) {

						//var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						var offsetFromDragPoint = intent.target.gridTargetPos - startDragMovePos;
						//Vector3 newPos = NoteGridSnap.SnapToGrid(mousePos, EditorInput.selectedSnappingMode);
						var tempNewPos = newPos + offsetFromDragPoint;
						intent.target.gridTargetIcon.transform.localPosition = new Vector3(tempNewPos.x, tempNewPos.y, intent.target.gridTargetPos.z);
						//target.gridTargetPos = target.gridTargetIcon.transform.localPosition;

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

						intent.intendedPosition = new Vector3(newPos.x, pos.y, pos.z);
					}
				}
			}
		}

		private Vector3 SnapToBeat(Vector3 position) {
			var increments = ((480 / timeline.beatSnap) * 4f) / 480;        // what even is life //42
			return new Vector3(
				Mathf.Round(position.x / increments) * increments,
				position.y,
				position.z
			);
		}
	}
}