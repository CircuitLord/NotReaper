using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class UndoRedoManager : MonoBehaviour {

		/// <summary>
		/// Contains the complete list of actions the user has done recently.
		/// </summary>
		public List<Action> actions = new List<Action>();

		/// <summary>
		/// Contains the actions the user has "undone" for future use.
		/// </summary>
		public List<Action> redoActions = new List<Action>();

		public int maxSavedActions = 20;
		




		public Timeline timeline;

		public void Undo() {
			if (actions.Count <= 0) return;

			Action action = actions.Last();
			Debug.Log("Undoing action:" + action.type.ToString());

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

			Action action = redoActions.Last();

			Debug.Log("Redoing action:" + action.type.ToString());

				RunAction(action);
			try {
			} catch {
				Debug.LogError("There was an error trying to redo.");
			}

			//We remove the old action and just add a new one from Timeline.cs since the old one lost the reference to it's GO's
			//AddAction(action, false);
			redoActions.RemoveAt(redoActions.Count - 1);
			

		}

		//Use for undo
		public void InvertAction(Action action) {

			switch (action.type) {
				
				case ActionType.AddNote:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(action.affectedTarget, false);
					break;

				case ActionType.MultiAddNote:
					foreach (Target target in action.affectedTargets) {
						timeline.DeleteTarget(target, false);
					}
					break;

				case ActionType.RemoveNote:
					//Re-add the target based on all the previous stats from the target;
					Target tar = action.affectedTarget;
					timeline.AddTarget(tar.gridTarget.transform.position.x, tar.gridTarget.transform.position.y, tar.gridTarget.transform.position.z, tar.gridTarget.beatLength, tar.gridTarget.velocity, tar.gridTarget.handType, tar.gridTarget.behavior, false);
					break;

				case ActionType.MultiRemoveNote:
					foreach (Target target in action.affectedTargets) {
						timeline.AddTarget(target.gridTarget.transform.position.x, target.gridTarget.transform.position.y, target.gridTarget.transform.position.z, target.gridTarget.beatLength, target.gridTarget.velocity, target.gridTarget.handType, target.gridTarget.behavior, false);
					}
					break;
				

				default:
					break;
				

			}


		}


		//Use for redo | calls functions that gen undo actions for the undo list.
		public void RunAction(Action action) {
			//TODO: Targets lose their transform when they get removed, so we can't restore it from the og transform values anymore
			switch (action.type) {
				case ActionType.AddNote:
					Target tar = action.affectedTarget;
					timeline.AddTarget(tar.gridTarget, true);
					break;

				case ActionType.MultiAddNote:
					foreach (Target target in action.affectedTargets) {
						timeline.AddTarget(target.gridTarget, true);
					}
					break;

				case ActionType.RemoveNote:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(action.affectedTarget, true);
					break;

				case ActionType.MultiRemoveNote:
					foreach (Target target in action.affectedTargets) {
						timeline.DeleteTarget(target, true);
					}
					break;




			}
		}



		public void AddAction(Action action, bool clearRedoActions = true) {
			if (actions.Count <= maxSavedActions) {
				actions.Add(action);
			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}

				actions.Add(action);
			}

			if (clearRedoActions) redoActions = new List<Action>();



		}






		
	}


	public enum ActionType {
		AddNote = 0,
		MultiAddNote = 1,
		RemoveNote = 2,
		MultiRemoveNote = 3,
		SelectNote = 4,
		MultiSelectNote = 5,
		DeselectNote = 6,
		MultiDeselectNote = 7,
		MoveNote = 8,
		MultiMoveNote = 9
		
	}

	public class Action {
		public ActionType type = ActionType.AddNote;
		public List<Target> affectedTargets = new List<Target>();
		public Target affectedTarget {
			get {
				return affectedTargets.First();
			}
		}

		public Vector2 redoTargetPos;

		public Vector2 movePreviousPos;

		

	}


}