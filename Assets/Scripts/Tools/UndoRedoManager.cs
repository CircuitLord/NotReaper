using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using NotReaper.Managers;

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

		public void ClearActions() {
			actions = new List<NRAction>();
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

	public class NRActionGridMoveNotes : NRAction {
		public List<TargetGridMoveIntent> targetGridMoveIntents = new List<TargetGridMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				intent.target.position = intent.intendedPosition;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				intent.target.position = intent.startingPosition;
			});
		}
	}

	public class NRActionTimelineMoveNotes : NRAction {
		public List<TargetTimelineMoveIntent> targetTimelineMoveIntents = new List<TargetTimelineMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				intent.target.beatTime = intent.intendedTime;
			});
			timeline.SortOrderedList();
		}
		public override void UndoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				intent.target.beatTime = intent.startTime;
			});
			timeline.SortOrderedList();
		}
	}

	public class NRActionSwapNoteColors : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				switch (targetData.handType) {
					case TargetHandType.Left: 
						targetData.handType = TargetHandType.Right;
					break;
					
					case TargetHandType.Right: 
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
				targetData.x *= -1;
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
				targetData.y *= -1;
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionSetTargetHitsound : NRAction {
		public List<TargetSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();

		public override void DoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				intent.target.velocity = intent.newVelocity;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				intent.target.velocity = intent.startingVelocity;
			});
		}
	}
}