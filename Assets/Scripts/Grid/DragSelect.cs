using UnityEngine;

namespace NotReaper.Grid {

	public class DragSelect : MonoBehaviour {

		bool isSelecting = false;
		Vector3 mousePosition1;

		void Update() {
			// If we press the left mouse button, save mouse location and begin selection
			if (Input.GetMouseButtonDown(0)) {
				isSelecting = true;
				mousePosition1 = Input.mousePosition;
			}
			// If we let go of the left mouse button, end selection
			if (Input.GetMouseButtonUp(0))
				isSelecting = false;
		}

		void OnGUI() {
			if (isSelecting) {
				// Create a rect from both mouse positions
				var rect = RectUtil.GetScreenRect(mousePosition1, Input.mousePosition);
				RectUtil.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
				RectUtil.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
			}
		}
	}
}