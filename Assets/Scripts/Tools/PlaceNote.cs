using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class PlaceNote : MonoBehaviour {

		public Timeline timeline;

		public Transform ghost;
		public LayerMask notesLayer;
		public NoteGridSnap noteSnap;

		public void TryPlaceNote() {
			if (!EditorInput.isOverGrid || EditorInput.inUI) return;
			
			//We check if the target is valid in the timeline function instead now.
			timeline.AddTarget(ghost.position.x, ghost.position.y);
		}

		public void TryRemoveNote() {
			var iconsUnderMouse = MouseUtil.IconsUnderMouse(timeline);
			TargetIcon targetIcon = iconsUnderMouse.Length > 0 ? iconsUnderMouse[0] : null;
			if (targetIcon) {
				targetIcon.OnTryRemove();
			}
		}
	}
}