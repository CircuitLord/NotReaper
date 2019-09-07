using System.Collections;
using UnityEngine;

namespace NotReaper.Tools.ChainBuilder {


	public class CurvedLinePoint : MonoBehaviour {
		[HideInInspector] public bool showGizmo = true;
		[HideInInspector] public float gizmoSize = 0.1f;
		[HideInInspector] public Color gizmoColor = new Color(1, 0, 0, 0.5f);

		[HideInInspector] public bool isChainStart = false;
		[HideInInspector] public bool isHovered = false;

		public GameObject chainStartIcon;
		public GameObject chainNodeIcon;

		public Animation chainNodeHoverAnim;

		public void MakeChainStart() {
			isChainStart = true;
			chainNodeIcon.SetActive(false);
			chainStartIcon.SetActive(true);
		}

		public void MakeChainNode() {
			isChainStart = false;
			chainNodeIcon.SetActive(true);
			chainStartIcon.SetActive(false);

		}

		// void OnDrawGizmos() {
		// 	if (showGizmo == true) {
		// 		Gizmos.color = gizmoColor;

		// 		Gizmos.DrawSphere(this.transform.position, gizmoSize);
		// 	}
		// }

		// //update parent line when this point moved
		// void OnDrawGizmosSelected() {
		// 	CurvedLineRenderer curvedLine = this.transform.parent.GetComponent<CurvedLineRenderer>();

		// 	if (curvedLine != null) {
		// 		curvedLine.Update();
		// 	}
		// }
	}

}