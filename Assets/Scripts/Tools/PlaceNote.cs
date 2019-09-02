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
			if (!EditorInput.isOverGrid) return;

			foreach (Target target in Timeline.selectableNotes) {
				if ((target.gridTargetIcon.transform.position.z == ghost.position.z) && (target.handType == EditorInput.selectedHand) && (EditorInput.selectedTool != EditorTool.Melee)) return;
			}

			timeline.AddTarget(ghost.position.x, ghost.position.y);
		}

		public void TryRemoveNote() {
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			TargetIcon targetIcon = IconUnderMouse();
			if (targetIcon) {
				targetIcon.OnTryRemove();
			}
		}




		private TargetIcon IconUnderMouse() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = new Vector3(ray.origin.x, ray.origin.y, -1.25f);
			ray.direction = Vector3.forward;
			Debug.DrawRay(ray.origin, ray.direction);
			if (Physics.Raycast(ray, out hit, 2.5f, notesLayer)) {
				Transform objectHit = hit.transform;

				TargetIcon targetIcon = objectHit.GetComponent<TargetIcon>();

				return targetIcon;
			}
			return null;
		}

	}
}