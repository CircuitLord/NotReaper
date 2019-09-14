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

			InvertAction(action);
			try {


			} catch {
				Debug.LogError("There was an error trying to undo.");
			}

			
			redoActions.Add(action);
			actions.RemoveAt(actions.Count - 1);

			
		}

		public void Redo() {

			if (redoActions.Count <= 0) return;

			NRAction action = redoActions.Last();

			Debug.Log("Redoing action:" + action.ToString());

				RunAction(action);
			try {
			} catch {
				Debug.LogError("There was an error trying to redo.");
			}

			//We remove the old action and just add a new one from Timeline.cs since the old one lost the reference to it's GO's
			//AddAction(action, false);
			redoActions.RemoveAt(redoActions.Count - 1);
			

		}

		//Use for undo (DON'T generate undo actions from these) and also DON'T generate redo actions (this is handled by the wrapper function for undoing)
		public void InvertAction(NRAction action) {

			switch (action) {
				
				case NRActionAddNote a:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(a.affectedTarget, false);
					break;

				case NRActionMultiAddNote a:
					timeline.DeleteTargets(a.affectedTargets, false);
					break;

				case NRActionRemoveNote a:
					//Re-add the target based on all the previous stats from the target;
					Target tar = a.affectedTarget;
					timeline.AddTarget(tar.gridTargetPos.x, tar.gridTargetPos.y, tar.gridTargetPos.z, false, false, tar.beatLength, tar.velocity, tar.handType, tar.behavior);
					break;

				case NRActionMultiRemoveNote a:
					timeline.AddTargets(a.affectedTargets, false);
					break;

				case NRActionPasteNotes a:
					timeline.DeleteTargets(a.pastedTargets, false);
					break;

				case NRActionGridMoveNotes a:
					var gridMoveIntents = new List<TargetMoveIntent>();
					a.targetGridMoveIntents.ForEach(intent => {
						var undoIntent = new TargetMoveIntent();
						undoIntent.target = intent.target;
						undoIntent.intendedPosition = intent.startingPosition;
						undoIntent.startingPosition = intent.intendedPosition;
						gridMoveIntents.Add(undoIntent);
					});
					timeline.MoveGridTargets(gridMoveIntents, false);
					break;

				case NRActionTimelineMoveNotes a:
					var timelineMoveIntents = new List<TargetMoveIntent>();
					a.targetTimelineMoveIntents.ForEach(intent => {
						var undoIntent = new TargetMoveIntent();
						undoIntent.target = intent.target;
						undoIntent.intendedPosition = intent.startingPosition;
						undoIntent.startingPosition = intent.intendedPosition;
						timelineMoveIntents.Add(undoIntent);
					});
					timeline.MoveTimelineTargets(timelineMoveIntents, false);
					break;

				case NRActionSwapNoteColors a:
					timeline.SwapTargets(a.affectedTargets, false);
					break;

				case NRActionHFlipNotes a:
					timeline.FlipTargetsHorizontal(a.affectedTargets, false);
					break;

				case NRActionVFlipNotes a:
					timeline.FlipTargetsVertical(a.affectedTargets, false);
					break;

				default:
					break;
			}
		}


		//Use for redo, DO generate undo actions, and DON'T clear redo actions
		public void RunAction(NRAction action) {
			switch (action) {
				case NRActionAddNote a:
					timeline.AddTarget(a.affectedTarget, true);
					break;

				case NRActionMultiAddNote a:
					timeline.AddTargets(a.affectedTargets, true);
					break;

				case NRActionRemoveNote a:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(a.affectedTarget, true, false);
					break;

				case NRActionMultiRemoveNote a:
					timeline.DeleteTargets(a.affectedTargets, true, false);
					break;

				case NRActionPasteNotes a:
					timeline.PasteCues(a.newCues, a.pasteBeatTime, true, false);
					break;

				case NRActionGridMoveNotes a:
					timeline.MoveGridTargets(a.targetGridMoveIntents, true, false);
					break;

				case NRActionTimelineMoveNotes a:
					timeline.MoveTimelineTargets(a.targetTimelineMoveIntents, true, false);
					break;

				case NRActionSwapNoteColors a:
					timeline.SwapTargets(a.affectedTargets, true, false);
					break;

				case NRActionHFlipNotes a:
					timeline.FlipTargetsHorizontal(a.affectedTargets, true, false);
					break;

				case NRActionVFlipNotes a:
					timeline.FlipTargetsVertical(a.affectedTargets, true, false);
					break;
			}
		}



		public void AddAction(NRAction action, bool clearRedoActions = true) {
			if (actions.Count <= maxSavedActions) {
				actions.Add(action);
			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}

				actions.Add(action);
			}

			if (clearRedoActions) redoActions = new List<NRAction>();

			//NRActionAddNote test = new NRActionAddNote();
			//redoActions.Add(test);



		}






		
	}


	// public enum ActionType {
	// 	AddNote = 0,
	// 	MultiAddNote = 1,
	// 	RemoveNote = 2,
	// 	MultiRemoveNote = 3,
	// 	SelectNote = 4,
	// 	MultiSelectNote = 5,
	// 	DeselectNote = 6,
	// 	MultiDeselectNote = 7,
	// 	MoveNote = 8,
	// 	MultiMoveNote = 9
		
	// }

	public class NRAction {
		//public ActionType type;
		//public List<Target> affectedTargets = new List<Target>();
		//public Target affectedTarget {
		//	get {
		//		return affectedTargets.First();
		//	}
		//}

		//public Vector2 redoTargetPos;

		//public Vector2 movePreviousPos;

		

	}

	public class NRActionAddNote : NRAction {
		public Target affectedTarget;
	}

	public class NRActionMultiAddNote : NRAction {
		public List<Target> affectedTargets = new List<Target>();
	}

	public class NRActionRemoveNote : NRAction {
		public Target affectedTarget;
	}
	
	public class NRActionMultiRemoveNote : NRAction {
		public List<Target> affectedTargets = new List<Target>();
	}

	public class NRActionPasteNotes : NRAction {
		public List<Cue> newCues = new List<Cue>();
		public List<Target> pastedTargets = new List<Target>();
		public float pasteBeatTime;
	}

	public class NRActionGridMoveNotes : NRAction {
		public List<TargetMoveIntent> targetGridMoveIntents = new List<TargetMoveIntent>();
	}

	public class NRActionTimelineMoveNotes : NRAction {
		public List<TargetMoveIntent> targetTimelineMoveIntents = new List<TargetMoveIntent>();
	}

	public class NRActionSwapNoteColors : NRAction {
		public List<Target> affectedTargets = new List<Target>();
	}

	public class NRActionHFlipNotes : NRAction {
		public List<Target> affectedTargets = new List<Target>();
	}

	public class NRActionVFlipNotes : NRAction {
		public List<Target> affectedTargets = new List<Target>();
	}
}