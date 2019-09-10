using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Tools {

	public class DragSelect : MonoBehaviour {


		public Timeline timeline;

		public bool activated = false;
		bool isDraggingTimeline = false;
		private float timelineStartDragPos = 0f;
		private float timelineStopDragPos = 0f;
		Vector3 mousePosition1;

		public static List<TargetIcon> selectedGridIcons = new List<TargetIcon>();
		public static List<TargetIcon> selectedTimelineIcons = new List<TargetIcon>();

		public Transform dragSelectTimeline;
		public Transform timelineNotes;








		void Update() {

			//if (!activated) return;

			

			//if hotkey pressed:
			if (Input.GetKeyDown(KeyCode.F)) {

				//If it's a timeline drag:
				if (timeline.hover) {

					

					float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

					dragSelectTimeline.SetParent(timelineNotes);
					dragSelectTimeline.position = new Vector3(mouseX, 0, 0);
					dragSelectTimeline.localPosition = new Vector3(dragSelectTimeline.transform.localPosition.x, 0.03f, 0);

					timelineStartDragPos = dragSelectTimeline.localPosition.x;

					dragSelectTimeline.gameObject.SetActive(true);

					isDraggingTimeline = true;

				}

			}


			if (Input.GetKeyUp(KeyCode.F)) {
				isDraggingTimeline = false;

				dragSelectTimeline.gameObject.SetActive(false);

				float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

				timelineStopDragPos = mouseX - timelineNotes.position.x;

				float timelineScaleMulti = timeline.scale / 20f;

				StartCoroutine(timeline.SelectNotesInTimelineBox(timelineStartDragPos * timelineScaleMulti, timelineStopDragPos * timelineScaleMulti));



			}


			if (isDraggingTimeline) {
				float diff = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - dragSelectTimeline.position.x;

				float timelineScaleMulti = timeline.scale / 20f;


				dragSelectTimeline.localScale = new Vector3(diff * timelineScaleMulti, 1.1f * (timeline.scale / 20f), 1);

			}


			// // If we press the left mouse button, save mouse location and begin selection
			// if (Input.GetKeyDown(KeyCode.F)) {
			// 	isSelecting = true;
			// 	mousePosition1 = Input.mousePosition;

			// 	//selectedGridIcons = new List<TargetIcon>();
			// 	//selectedTimelineIcons = new List<TargetIcon>();
			// 	foreach (GridTarget target in Timeline.orderedNotes) {
			// 		target.Deselect();
			// 	}
			// }
			// // If we let go of the left mouse button, end selection
			// if (Input.GetKeyUp(KeyCode.F)) {


			// 	foreach (GridTarget target in Timeline.selectableNotes) {

			// 		if (target == null) return;
			// 	//FIXME: selecting notes
			// 		//if (IsWithinSelectionBounds(target.gameObject)) {
			// 		//	target.Select();
			// 		//	selectedTargets.Add(target);
			// 		//}
			// 	}


			// 	isSelecting = false;
			// }


		}



		

		void OnGUI() {
			return;
			//if (isSelecting) {
				// Create a rect from both mouse positions
				var rect = DragUtil.GetScreenRect(mousePosition1, Input.mousePosition);
				DragUtil.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
				DragUtil.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
			//}


		}


		public bool IsWithinSelectionBounds(GameObject gameObject) {
			//if (!isSelecting)
				return false;

			var camera = Camera.main;
			var viewportBounds =
				DragUtil.GetViewportBounds(camera, mousePosition1, Input.mousePosition);

			return viewportBounds.Contains(
				camera.WorldToViewportPoint(gameObject.transform.position));
		}


	}
}