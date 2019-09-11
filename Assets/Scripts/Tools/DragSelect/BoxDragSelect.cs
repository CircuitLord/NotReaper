using System.Collections;
using System.Collections.Generic;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Tools {


    public class BoxDragSelect : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
			Debug.Log(other.name);
			var targetIcon = other.GetComponent<TargetIcon>();
			if (targetIcon) {
				targetIcon.TrySelect();
			}
		}

		private void OnTriggerExit(Collider other) {
			var targetIcon = other.GetComponent<TargetIcon>();
			if (targetIcon) {
				targetIcon.TryDeselect();
			}
		}
    }

}