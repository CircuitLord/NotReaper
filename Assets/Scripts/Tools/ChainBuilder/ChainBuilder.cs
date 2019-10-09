using System;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Tools.ChainBuilder {


	public class ChainBuilder : MonoBehaviour {


		public GameObject curvedLinePrefab;
		public GameObject linePointPrefab;
		public GameObject nodeTempPointPrefab;

		[SerializeField] private Transform chainBuilderLinesParent;
		private Transform tempNodeIconsParent;

		public LayerMask chainBuilderLayer;

		public bool isDragging = false;
		public bool isEditMode = false;
		public bool active = false;

		private Transform draggingPoint;
		public GameObject activeChain;

		private int prevDrawPointsAmt = 0;



		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate() {
			active = true;

			var startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			NewChain(startPos);


		}

		public void Deactivate() {
			active = false;
		}
		

		/// <summary>
		/// Sets the tool to be in select mode.
		/// </summary>
		public void SelectMode() {

			//TODO: Check if we want to apply or save our current chain
			if (activeChain) {

			}

			isEditMode = false;
		}

		public void EditMode() {
			//TODO: Find which chain we clicked on to trigger edit mode
			isEditMode = true;
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
			
			tempNodeIconsParent = activeChain.transform.Find("TempNodeIcons");


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
			
			if (!active) return;




			if (Input.GetMouseButtonDown(0) && EditorInput.isOverGrid) {
				Transform point = FindLinePointUnderMouse();

				if (point) {
					draggingPoint = point;
					isDragging = true;

				} else {
					draggingPoint = AddPointToActive(Camera.main.ScreenToWorldPoint(Input.mousePosition), false).transform;
					isDragging = true;
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

				DrawTempChain();
				
			}

			if (!Input.GetMouseButton(0)) isDragging = false;


		}


		public void DrawTempChain() {
			List<Vector2> points = FindPointsAlongChain(10);
			if (points != null) DrawPointsAlongChain(points);
		}




		private void DrawPointsAlongChain(List<Vector2> points) {

			RemoveTempPointsAlongChain();

			foreach (Vector2 point in points) {
				Instantiate(nodeTempPointPrefab, new Vector3(point.x, point.y, 0f), Quaternion.identity, tempNodeIconsParent);
			}
		}

		private void RemoveTempPointsAlongChain() {
			foreach (Transform child in tempNodeIconsParent) {
				Destroy(child.gameObject);
			}
		}


		private List<Vector2> FindPointsAlongChain(int noteCount) {

			if (!activeChain) return null;

			//If we need to draw a new amount of notes or something else
			//if (noteCount == prevDrawPointsAmt) return null;

			prevDrawPointsAmt = noteCount;

			var lr = activeChain.GetComponent<LineRenderer>();
			int pCount = lr.positionCount;

			//If the amount of points on the line is lower than the positions we want to get, return to prevent crash.
			if (pCount < noteCount) return null;
			
			double indexDist = pCount / noteCount;


			List<Vector2> points = new List<Vector2>();

			for (double i = indexDist; i < pCount; i += indexDist) {
				//Get the previous index
				Vector3 pos1 = lr.GetPosition((int)Math.Floor(i));
				Vector3 pos2 = lr.GetPosition((int)Math.Ceiling(i));

				//4.3 -> 0.3
				double firstOffsetFromIndex = i - Math.Floor(i);
				//4.3 -> 0.7
				double secondOffsetFromIndex = Math.Ceiling(i) - i;

				if (firstOffsetFromIndex == 0) firstOffsetFromIndex = 1;

				Vector2 avg1 = pos1 * (float)firstOffsetFromIndex;
				Vector2 avg2 = pos2 * (float)secondOffsetFromIndex;

				Vector2 final = avg1 + avg2;
				points.Add(final);
			}

			var thing = points.Count;

			return points;
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