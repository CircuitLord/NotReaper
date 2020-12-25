using System;
using System.Collections.Generic;
using System.Linq;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using NotReaper.Timing;
using NotReaper.Modifier;

namespace NotReaper.Tools {
	public class DragSelect : MonoBehaviour {
		public Timeline timeline;
		public Transform dragSelectTimeline;
		public Transform timelineNotes;
		public GameObject dragSelectGrid;
		public LayerMask notesLayer;

		private bool activated = false;

		private bool isDraggingTimeline = false;
		private List<Target> dragTimelineSelectedTargets = new List<Target>();

		private bool isDraggingGrid = false;
		private List<Target> dragGridSelectedTarget = new List<Target>();

		private bool isDraggingNotesOnGrid = false;
		private bool isDraggingNotesOnTimeline = false;

		// TODO: This should come from whatever new input handler we end up with, for now this at least should abstract input from intent (see CaptureInput())
		// Represent action intents occuring this update frame.
		private bool frameIntentSelect = false; // click + release
		private bool frameIntentDragStart = false; // click + drag start frame
		private bool frameIntentDragging = false; // click + drag frame
		private bool frameIntentDragEnd = false; // click + drag end frame
		private bool frameIntentCut = false;
		private bool frameIntentCopy = false;
		private bool frameIntentPaste = false;
		private bool frameIntentDelete = false;
		private bool frameIntentDeselectAll = false;
		private bool frameIntentSwapColors = false;
		private bool frameIntentFlipTargetsHorizontally = false;
		private bool frameIntentFlipTargetsVertically = false;
		private bool frameIntentScaleUp = false;
		private bool frameIntentReverse = false;
		private bool frameIntentRotateLeft = false;
		private bool frameIntentRotateRight = false;
		private bool frameIntentScaleDown = false;
		private bool frameIntentSetHitSoundStandard = false;
		private bool frameIntentSetHitSoundSnare = false;
		private bool frameIntentSetHitSoundPercussion = false;
		private bool frameIntentSetHitSoundChainStart = false;
		private bool frameIntentSetHitSoundChain = false;
		private bool frameIntentSetHitSoundMelee = false;

		//Change note behavior
		private TargetBehavior frameIntentSetBehavior = TargetBehavior.None;

		private bool hasInputDragMovedOutOfClickBounds = false;
		private bool dragStarted = false;

		private TargetIcon[] _iconsUnderMouse = new TargetIcon[0];

		private Vector2 startGridMovePos;
		private Vector2 startClickDetectPos;
		private QNT_Timestamp startTimelineMoveTime;

		private List<TargetData> clipboardNotes = new List<TargetData>();
		private List<TargetGridMoveIntent> gridTargetMoveIntents = new List<TargetGridMoveIntent>();
		private List<TargetTimelineMoveIntent> timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
		private List<TargetSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();

		/** Fetch all icons currently under the mouse
		 *  Will only ever happen once per frame */
		public TargetIcon[] iconsUnderMouse {
			get {
				return _iconsUnderMouse = _iconsUnderMouse == null
					? MouseUtil.IconsUnderMouse(timeline)
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
		}

		public void Update() {
			CaptureInput();
			UpdateActions();
			UpdateSelections();
			UpdateDragging();
			iconsUnderMouse = null;
		}

		public void EndAllDragStuff() {
			if (isDraggingNotesOnTimeline) {
				EndDragTimelineTargetAction();
			}
			else if (isDraggingNotesOnGrid) {
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
			dragTimelineSelectedTargets = new List<Target>();
		}

		private void UpdateTimelineSelection() {
			float diff = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - dragSelectTimeline.position.x;
			float timelineScaleMulti = Timeline.scale / 20f;
			dragSelectTimeline.localScale = new Vector3(diff * timelineScaleMulti, 1.1f, 1);

			Vector2 topLeft = dragSelectTimeline.transform.TransformPoint(0, 0, 0);
			Vector2 size = dragSelectTimeline.transform.TransformVector(1, 1, 1);

			Vector2 center = new Vector2(topLeft.x + size.x / 2, topLeft.y - size.y / 2);

			float minX = Math.Min(center.x - size.x / 2, center.x + size.x / 2);
			float maxX = Math.Max(center.x - size.x / 2, center.x + size.x / 2);
			float minY = Math.Min(center.y - size.y / 2, center.y + size.y / 2);
			float maxY = Math.Max(center.y - size.y / 2, center.y + size.y / 2);

			Rect selectionRect = Rect.MinMaxRect(minX, minY, maxX, maxY);

			float offscreenOffset = timelineNotes.parent.position.x;
			QNT_Timestamp start = Timeline.time + Relative_QNT.FromBeatTime((minX - offscreenOffset - 1.0f) * timelineScaleMulti);
			QNT_Timestamp end = Timeline.time + Relative_QNT.FromBeatTime((maxX - offscreenOffset + 1.0f) * timelineScaleMulti);
			if(start > end) {
				QNT_Timestamp temp = start;
				start = end;
				end = temp;
			}

			List<Target> newSelectedTargets = new List<Target>();
			foreach(Target target in new NoteEnumerator(start, end)) {
				if(target.IsTimelineInsideRect(selectionRect)) {
					newSelectedTargets.Add(target);
				}
			}

			var deselectedTargets = dragTimelineSelectedTargets.Except(newSelectedTargets);
			foreach(Target t in deselectedTargets) {
				t.MakeTimelineDeselectTarget();
			}

			var selectedTargets = newSelectedTargets.Except(dragTimelineSelectedTargets);
			foreach(Target t in selectedTargets) {
				t.MakeTimelineSelectTarget();
			}

			dragTimelineSelectedTargets = newSelectedTargets;
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
			dragGridSelectedTarget = new List<Target>();
		}

		private void UpdateGridSelection() {
			Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
			               dragSelectGrid.transform.position;
			dragSelectGrid.transform.localScale = new Vector3(diff.x, diff.y * -1, 1f);

			Vector2 topLeft = new Vector2(dragSelectGrid.transform.position.x, dragSelectGrid.transform.position.y);
			Vector2 size = new Vector2(dragSelectGrid.transform.localScale.x, dragSelectGrid.transform.localScale.y);

			Vector2 center = new Vector2(topLeft.x + size.x / 2, topLeft.y - size.y / 2);

			float minX = Math.Min(center.x - size.x / 2, center.x + size.x / 2);
			float maxX = Math.Max(center.x - size.x / 2, center.x + size.x / 2);
			float minY = Math.Min(center.y - size.y / 2, center.y + size.y / 2);
			float maxY = Math.Max(center.y - size.y / 2, center.y + size.y / 2);

			Rect selectionRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
			List<Target> newSelectedTargets = new List<Target>();
			foreach(Target t in Timeline.loadedNotes) {
				if(t.IsInsideRectAtTime(Timeline.time, selectionRect)) {
					newSelectedTargets.Add(t);
				}
			}

			var deselectedTargets = dragGridSelectedTarget.Except(newSelectedTargets);
			foreach(Target t in deselectedTargets) {
				t.MakeTimelineDeselectTarget();
			}

			var selectedTargets = newSelectedTargets.Except(dragGridSelectedTarget);
			foreach(Target t in selectedTargets) {
				t.MakeTimelineSelectTarget();
			}

			dragGridSelectedTarget = newSelectedTargets;
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
			startTimelineMoveTime = icon.data.time;
			timelineTargetMoveIntents = new List<TargetTimelineMoveIntent>();
			timeline.selectedNotes.ForEach(target => {
				var intent = new TargetTimelineMoveIntent();
				intent.targetData = target.data;
				intent.startTick = target.data.time;

				timelineTargetMoveIntents.Add(intent);
			});
		}

		private void UpdateDragTimelineTargetAction() {
			foreach (TargetTimelineMoveIntent intent in timelineTargetMoveIntents) {
				Relative_QNT offsetFromDragPoint = intent.startTick - startTimelineMoveTime;
				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePos.x /= Timeline.scaleTransform;
				mousePos.x -= timelineNotes.parent.position.x;
				QNT_Timestamp newTime = SnapToBeat(mousePos.x);

				newTime += offsetFromDragPoint;
				intent.targetData.SetTimeFromAction(newTime);
				intent.intendedTick = newTime;
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
			if (EditorInput.selectedTool != EditorTool.DragSelect) return;

			if(NRSettings.config.singleSelectCtrl) {
				bool addToSelection = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

				if(!addToSelection) {
					timeline.DeselectAllTargets();
				}
			}
			
			if (iconUnderMouse && iconUnderMouse.isSelected) {
				iconUnderMouse.TryDeselect();
			}
			else if (iconUnderMouse && !iconUnderMouse.isSelected) {
				iconUnderMouse.TrySelect();
			}
			else {
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

		private void SetBehaviorAction(TargetBehavior behavior) {
			NRActionSetTargetBehavior setBehaviorAction = new NRActionSetTargetBehavior();
			setBehaviorAction.newBehavior = behavior;
			timeline.selectedNotes.ForEach(target => {
				setBehaviorAction.affectedTargets.Add(target.data);
			});

			timeline.SetTargetBehaviors(setBehaviorAction);
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
			frameIntentReverse = false;
			frameIntentRotateLeft = false;
			frameIntentRotateRight = false;
			frameIntentScaleUp = false;
			frameIntentScaleDown = false;
			
			frameIntentSetBehavior = TargetBehavior.None;

			// Keyboard input
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) primaryModifierHeld = true;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) secondaryModifierHeld = true;

			if (primaryModifierHeld && !secondaryModifierHeld && !ModifierHandler.activated && !BookmarkMenu.isActive) {
				// permits holding primary and secondary down
				frameIntentFlipTargetsHorizontally = Input.GetKeyDown(KeyCode.F);
				frameIntentCut = Input.GetKeyDown(KeyCode.X);
				frameIntentCopy = Input.GetKeyDown(KeyCode.C);
				frameIntentPaste = Input.GetKeyDown(KeyCode.V);
				frameIntentDeselectAll = Input.GetKeyDown(KeyCode.D);
				frameIntentReverse = Input.GetKeyDown(KeyCode.G);
				frameIntentRotateLeft = Input.GetKeyDown(KeyCode.Minus);
				frameIntentRotateRight = Input.GetKeyDown(KeyCode.Equals);

				if(Input.GetKeyDown(InputManager.selectStandard)) {
					frameIntentSetBehavior = TargetBehavior.Standard;
				}
				else if(Input.GetKeyDown(InputManager.selectChainNode)) {
					frameIntentSetBehavior = TargetBehavior.Chain;
				}
				else if(Input.GetKeyDown(InputManager.selectChainStart)) {
					frameIntentSetBehavior = TargetBehavior.ChainStart;
				}
				else if(Input.GetKeyDown(InputManager.selectHold)) {
					frameIntentSetBehavior = TargetBehavior.Hold;
				}
				else if(Input.GetKeyDown(InputManager.selectHorz)) {
					frameIntentSetBehavior = TargetBehavior.Horizontal;
				}
				else if(Input.GetKeyDown(InputManager.selectMelee)) {
					frameIntentSetBehavior = TargetBehavior.Melee;
				}
				else if(Input.GetKeyDown(InputManager.selectMine)) {
					frameIntentSetBehavior = TargetBehavior.Mine;
				}
				else if(Input.GetKeyDown(InputManager.selectVert)) {
					frameIntentSetBehavior = TargetBehavior.Vertical;
				}
			}
			else if (secondaryModifierHeld) {
				frameIntentFlipTargetsVertically = Input.GetKeyDown(KeyCode.F);
				frameIntentScaleUp = Input.GetKeyDown(KeyCode.Equals);
				frameIntentScaleDown = Input.GetKeyDown(KeyCode.Minus);
				
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


			//Handle note movement by arrow keys
			Vector2 noteMovement = new Vector2(0,0);
			noteMovement.x += Input.GetKeyDown(KeyCode.RightArrow) ? NotePosCalc.xSize : 0;
			noteMovement.x += Input.GetKeyDown(KeyCode.LeftArrow) ? -NotePosCalc.xSize : 0;
			noteMovement.y += Input.GetKeyDown(KeyCode.UpArrow) ? NotePosCalc.ySize : 0;
			noteMovement.y += Input.GetKeyDown(KeyCode.DownArrow) ? -NotePosCalc.ySize : 0;

			if(noteMovement.x != 0 || noteMovement.y != 0) {
				if(primaryModifierHeld) {
					noteMovement /= 2;
				}
				if(secondaryModifierHeld) {
					noteMovement /= 4;
				}

				timeline.MoveGridTargets(timeline.selectedNotes.Select(target => {
					var intent = new TargetGridMoveIntent();
					intent.target = target.data;
					intent.startingPosition = new Vector2(target.data.x, target.data.y);
					intent.intendedPosition = new Vector2(target.data.x + noteMovement.x, target.data.y + noteMovement.y);
					return intent;
				}).ToList());
			}
		}

		// execute simple actions which don't require any state management'
		private void UpdateActions() {

            if (ModifierHandler.activated || BookmarkMenu.isActive) return;
			//if (EditorInput.selectedTool != EditorTool.DragSelect) return;
			/** Setting hitsounds **/
			if (frameIntentSetHitSoundStandard) SetHitsoundAction(TargetVelocity.Standard);
			if (frameIntentSetHitSoundSnare) SetHitsoundAction(TargetVelocity.Snare);
			if (frameIntentSetHitSoundPercussion) SetHitsoundAction(TargetVelocity.Percussion);
			if (frameIntentSetHitSoundChain) SetHitsoundAction(TargetVelocity.Chain);
			if (frameIntentSetHitSoundChainStart) SetHitsoundAction(TargetVelocity.ChainStart);
			if (frameIntentSetHitSoundMelee) SetHitsoundAction(TargetVelocity.Melee);

			if(frameIntentSetBehavior != TargetBehavior.None) {
				SetBehaviorAction(frameIntentSetBehavior);
			}

			/** Cut Copy Paste Delete **/
			if (frameIntentCut) {
				frameIntentCopy = true;
				frameIntentDelete = true;
			}

			if (frameIntentCopy) {
				clipboardNotes = new List<TargetData>();
				timeline.selectedNotes.ForEach(note => clipboardNotes.Add(note.data));
			}

			if (frameIntentPaste) {
				timeline.DeselectAllTargets();
				timeline.PasteCues(clipboardNotes, Timeline.time);
			}

			if (frameIntentDelete) {
				if (timeline.selectedNotes.Count > 0) {
					timeline.DeleteTargets(timeline.selectedNotes);
				}

				timeline.selectedNotes = new List<Target>();
			}

			if (frameIntentReverse) timeline.Reverse(timeline.selectedNotes);
			
			/** Note flipping **/
			if (frameIntentSwapColors) timeline.SwapTargets(timeline.selectedNotes);
			if (frameIntentFlipTargetsHorizontally) timeline.FlipTargetsHorizontal(timeline.selectedNotes);
			if (frameIntentFlipTargetsVertically) timeline.FlipTargetsVertical(timeline.selectedNotes);
			
			/** Rotate notes **/
			if (frameIntentRotateLeft) timeline.Rotate(timeline.selectedNotes, 15);
			if (frameIntentRotateRight) timeline.Rotate(timeline.selectedNotes, -15);

			/** Scale notes **/
			if (frameIntentScaleUp) timeline.Scale(timeline.selectedNotes, 1.1f);
			if (frameIntentScaleDown) timeline.Scale(timeline.selectedNotes, 0.9f);

			/** Note selection and movement **/
			if (frameIntentDeselectAll && !ModifierHandler.activated) timeline.DeselectAllTargets();
		}

		// Update the selection box and individually added / removed targets
		private void UpdateSelections() {

			if (EditorInput.selectedTool != EditorTool.DragSelect) return;
			
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
					bool anySelectedIconUnderMouse =
						iconsUnderMouse.Where(icon => icon.isSelected).ToArray().Length != 0;
					if (iconUnderMouse && !anySelectedIconUnderMouse) {
						TryToggleSelection();
						anySelectedIconUnderMouse = true;
					}

					if (anySelectedIconUnderMouse) {
						if (iconUnderMouse.location == TargetIconLocation.Grid)
							StartDragGridTargetAction(iconUnderMouse);
						if (iconUnderMouse.location == TargetIconLocation.Timeline)
							StartDragTimelineTargetAction(iconUnderMouse);
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

		private QNT_Timestamp SnapToBeat(float posX) {
			QNT_Timestamp time = Timeline.time + Relative_QNT.FromBeatTime(posX);
			return timeline.GetClosestBeatSnapped(time + Constants.DurationFromBeatSnap((uint)timeline.beatSnap) / 2, (uint)timeline.beatSnap);
		}
	}
}