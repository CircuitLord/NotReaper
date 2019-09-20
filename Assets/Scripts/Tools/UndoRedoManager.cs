using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class UndoRedoManager : MonoBehaviour {

		/// <summary>
		/// Contains the complete list of actions the user has done recently.
		/// </summary>
		public List<NRAction> actions = new List<NRAction>();

		/// <summary>
		/// Contains the actions the user has "undone" for future use.
		/// </summary>
		public List<NRAction> redoActions = new List<NRAction>();

		public int maxSavedActions = 20;

		public Timeline timeline;

		public void Undo() {
			if (actions.Count <= 0) return;

			NRAction action = actions.Last();
			Debug.Log("Undoing action:" + action.ToString());

			action.UndoAction(timeline);
			
			redoActions.Add(action);
			actions.RemoveAt(actions.Count - 1);

			
		}

		public void Redo() {

			if (redoActions.Count <= 0) return;

			NRAction action = redoActions.Last();

			Debug.Log("Redoing action:" + action.ToString());

			action.DoAction(timeline);

			actions.Add(action);
			redoActions.RemoveAt(redoActions.Count - 1);
		}

		public void AddAction(NRAction action) {
			action.DoAction(timeline);

			if (actions.Count <= maxSavedActions) {
				actions.Add(action);
			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}

				actions.Add(action);
			}

			redoActions = new List<NRAction>();
		}
	}

	public abstract class NRAction {
		public abstract void DoAction(Timeline timeline);
		public abstract void UndoAction(Timeline timeline);
	}

	public class NRActionAddNote : NRAction {
		public TargetData targetData;

		public override void DoAction(Timeline timeline) {
			timeline.AddTargetFromAction(targetData);
		}
		public override void UndoAction(Timeline timeline) {
			timeline.DeleteTargetFromAction(targetData);
		}
	}

	public class NRActionMultiAddNote : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.AddTargetFromAction(targetData); });
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.DeleteTargetFromAction(targetData); });
		}
	}
	
	public class NRActionRemoveNote : NRAction {
		public TargetData targetData;

		public override void DoAction(Timeline timeline) {
			timeline.DeleteTargetFromAction(targetData);
		}
		public override void UndoAction(Timeline timeline) {
			timeline.AddTargetFromAction(targetData);
		}
	}
	
	public class NRActionMultiRemoveNote : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.DeleteTargetFromAction(targetData); });
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.AddTargetFromAction(targetData); });
		}
	}

	public class TargetDataMoveIntent {
		public TargetDataMoveIntent() {}

		public TargetDataMoveIntent(TargetMoveIntent intent) {
			targetData = new TargetData(intent.target);
			startingPosition = intent.startingPosition;
			intendedPosition = intent.intendedPosition;
		}

		public TargetData targetData;
		public Vector3 startingPosition;
		public Vector3 intendedPosition;
	}

	public class NRActionGridMoveNotes : NRAction {
		public List<TargetDataMoveIntent> targetGridMoveIntents = new List<TargetDataMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				target.gridTargetIcon.transform.localPosition = intent.intendedPosition;
				intent.targetData.x = intent.intendedPosition.x;
				intent.targetData.y = intent.intendedPosition.y;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				target.gridTargetIcon.transform.localPosition = intent.startingPosition;
				intent.targetData.x = intent.startingPosition.x;
				intent.targetData.y = intent.startingPosition.y;
			});
		}
	}

	public class NRActionTimelineMoveNotes : NRAction {
		public List<TargetDataMoveIntent> targetTimelineMoveIntents = new List<TargetDataMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				var newPos = intent.intendedPosition;

				var gridPos = target.gridTargetIcon.transform.localPosition;
				target.timelineTargetIcon.transform.localPosition = newPos;
				target.gridTargetIcon.transform.localPosition = new Vector3(gridPos.x, gridPos.y, newPos.x);
				intent.targetData.beatTime = newPos.x;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				var newPos = intent.startingPosition;

				var gridPos = target.gridTargetIcon.transform.localPosition;
				target.timelineTargetIcon.transform.localPosition = newPos;
				target.gridTargetIcon.transform.localPosition = new Vector3(gridPos.x, gridPos.y, newPos.x);
				intent.targetData.beatTime = newPos.x;
			});
		}
	}

	public class NRActionSwapNoteColors : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				Target target = timeline.FindNote(targetData);
				switch (target.handType) {
					case TargetHandType.Left: 
						target.SetHandType(TargetHandType.Right); 
						targetData.handType = TargetHandType.Right;
					break;
					
					case TargetHandType.Right: 
						target.SetHandType(TargetHandType.Left);
						targetData.handType = TargetHandType.Left;
					break;
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionHFlipNotes : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				Target target = timeline.FindNote(targetData);
				var pos = target.gridTargetIcon.transform.localPosition;
				target.gridTargetIcon.transform.localPosition = new Vector3(
					pos.x * -1,
					pos.y,
					pos.z
				);
				target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
				targetData.x = target.gridTargetPos.x;
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionVFlipNotes : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				Target target = timeline.FindNote(targetData);
				var pos = target.gridTargetIcon.transform.localPosition;
				target.gridTargetIcon.transform.localPosition = new Vector3(
					pos.x,
					pos.y * -1,
					pos.z
				);
				target.gridTargetPos = target.gridTargetIcon.transform.localPosition;
				targetData.y = target.gridTargetPos.y;
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}
	
	public class TargetDataSetHitsoundIntent {
		public TargetDataSetHitsoundIntent() {}

		public TargetDataSetHitsoundIntent(TargetSetHitsoundIntent intent) {
			targetData = new TargetData(intent.target);
			startingVelocity = intent.startingVelocity;
			newVelocity = intent.newVelocity;
		}

		public TargetData targetData;
		public TargetVelocity startingVelocity;
		public TargetVelocity newVelocity;
	}
	
	public class NRActionSetTargetHitsound : NRAction {
		public List<TargetDataSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetDataSetHitsoundIntent>();

		public override void DoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				target.SetVelocity(intent.newVelocity);
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				Target target = timeline.FindNote(intent.targetData);
				target.SetVelocity(intent.startingVelocity);
			});
		}
	}
}