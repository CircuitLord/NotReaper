using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class UndoRedoManager : MonoBehaviour {

		public List<Action> actions = new List<Action>();

		public int maxSavedActions = 20;


		public Timeline timeline;

		public void Undo() {
			
		}

		public void Redo() {

		}


		public void AddAction(Action action) {

			if (actions.Count <= maxSavedActions) {
				actions.Add(action);

			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}
			}

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
		public List<GridTarget> affectedGridTargets = new List<GridTarget>();
		public GridTarget affectedGridTarget {
			get {
				return affectedGridTargets.First();
			}
		}
		public Vector2 movePreviousPos;

		

	}


}