using NotReaper.Tools;
using NotReaper.Tools.ChainBuilder;
using UnityEngine;


namespace NotReaper.Managers {




	public class EditorToolkit : MonoBehaviour {

		

		[SerializeField] public PlaceNote placeNote;

		[SerializeField] public UndoRedoManager undoRedoManager;

		public ChainBuilder chainBuilder;

		public DragSelect dragSelect;



	}


}