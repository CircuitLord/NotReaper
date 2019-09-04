using System;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Tools.ChainBuilder {


	public class ChainBuilder : MonoBehaviour {

		public Timeline timeline;

		public GameObject curvedLinePrefab;
		public GameObject linePointPrefab;

		public Transform chainBuilderLinesParent;

		public LayerMask chainBuilderLayer;

		public bool isDragging = false;
		private Transform draggingPoint;

		public GameObject activeChain;

		bool activated = false;



		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate(bool active) {
			activated = active;


		}
		



		public void NewChain(Vector2 p1) {
			Vector3 startPos = new Vector3(p1.x, p1.y, 0f);
			activeChain = Instantiate(curvedLinePrefab, startPos, Quaternion.identity, chainBuilderLinesParent);

			AddPointToActive(startPos, true);
			
			//Spawn the first point somewhere safe in the grid.
			Vector3 pos2 = new Vector3(p1.x, p1.y, 0f);

			if (p1.x >= 0) {
				pos2.x -= 2f;
			} else {
				pos2.x += 2;
			}
			if (p1.y >= 0) {
				pos2.y -= 2f;
			} else {
				pos2.y += 2;
			}

			AddPointToActive(pos2, false);
			


		}


		public GameObject AddPointToActive(Vector3 pos, bool isChainStart = false) {
			var point = Instantiate(linePointPrefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, activeChain.transform);
			if (isChainStart)
				point.GetComponent<CurvedLinePoint>().MakeChainStart();
			else
				point.GetComponent<CurvedLinePoint>().MakeChainNode();

			return point;
		}


		private void Update() {

			if (!activated) return;

			if (Input.GetMouseButtonDown(0) && EditorInput.isOverGrid) {
				Transform point = FindLinePointUnderMouse();

				if (point) {
					draggingPoint = point;
					isDragging = true;

				} else {
					AddPointToActive(Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
				}
			}

			if (Input.GetMouseButtonDown(1) && EditorInput.isOverGrid) {
				Transform point = FindLinePointUnderMouse();
				if (point && !point.GetComponent<CurvedLinePoint>().isChainStart) {
					Destroy(point.gameObject);
				}
			}

			
			if (isDragging) {
				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				draggingPoint.position = new Vector3(mousePos.x, mousePos.y, 0);
			}

			if (!Input.GetMouseButton(0)) isDragging = false;


		}





		private Transform FindLinePointUnderMouse() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = new Vector3(ray.origin.x, ray.origin.y, -1.25f);
			ray.direction = Vector3.forward;
			Debug.DrawRay(ray.origin, ray.direction);
			if (Physics.Raycast(ray, out hit, 2.5f, chainBuilderLayer)) {
				Transform objectHit = hit.transform;
				
				return objectHit;
			}
			return null;
		}


	}


}