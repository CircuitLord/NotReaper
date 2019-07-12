using NotReaper.Targets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class PlaceNote : MonoBehaviour {

		Timeline timeline;

		public Transform ghost;
		public LayerMask notesLayer;


		bool hover;


		public void TryPlaceNote() {
			if (!hover) return;
			timeline.AddTarget(ghost.position.x, ghost.position.y);
		}

		public void TryRemoveNote() {
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			timeline.DeleteTarget(NoteUnderMouse());
		}


		private void OnMouseEnter() {
			hover = true;
			ghost.gameObject.SetActive(true);
		}

		private void OnMouseExit() {
			hover = false;
			ghost.gameObject.SetActive(false);
		}

		private Target NoteUnderMouse() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = new Vector3(ray.origin.x, ray.origin.y, -1f);
			ray.direction = Vector3.forward;
			Debug.DrawRay(ray.origin, ray.direction);
			if (Physics.Raycast(ray, out hit, 2, notesLayer)) {
				Transform objectHit = hit.transform;

				Target target = objectHit.GetComponent<Target>().gridTarget;


				return target;
			}
			return null;
		}

	}
}