using System;
using System.Collections.Generic;
using System.Linq;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using DG.Tweening;

namespace NotReaper.Tools.ChainBuilder { 

	public class ChainBuilder : MonoBehaviour {
		public LayerMask notesLayer;

		private TargetIcon[] _iconsUnderMouse = new TargetIcon[0];

		/** Fetch all icons currently under the mouse
		 *  Will only ever happen once per frame */
		public TargetIcon[] iconsUnderMouse {
			get {
				return _iconsUnderMouse = _iconsUnderMouse == null
					? MouseUtil.IconsUnderMouse(notesLayer)
					: _iconsUnderMouse;
			}
			set { _iconsUnderMouse = value; }
		}

		// Fetch the highest priority target (closest to current time)
		public TargetIcon iconUnderMouse {
			get {
				return iconsUnderMouse != null && iconsUnderMouse.Length > 0
					? iconsUnderMouse[0]
					: null;
			}
		}


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



		[SerializeField] private GameObject chainBuilderWindow; 

		void Start() {
			Vector3 defaultPos;
			defaultPos.x = 289.0f;
			defaultPos.y = -92.2f;
			defaultPos.z = -10.0f;

			chainBuilderWindow.GetComponent<RectTransform>().localPosition = defaultPos;
			chainBuilderWindow.GetComponent<CanvasGroup>().alpha = 0.0f;
		}

		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate(bool active) {
			activated = active;

			if(active) {
				chainBuilderWindow.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
			}
			else {
				chainBuilderWindow.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
			}
		}
		

		/// <summary>
		/// Sets the tool to be in select mode.
		/// </summary>
		public void SelectMode() {
			isEditMode = false;
		}

		public void EditMode() {
			//TODO: Find which chain we clicked on to trigger edit mode
			isEditMode = true;
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

			if (Input.GetMouseButtonDown(0)) {
				if(iconUnderMouse != null) {
					iconUnderMouse.data.behavior = TargetBehavior.NR_Pathbuilder;
				}
			}

			iconsUnderMouse = null;
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