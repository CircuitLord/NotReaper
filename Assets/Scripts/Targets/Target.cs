using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Targets {

	//Stores references to a timelineTarget and it's corrosponding gridTarget
	public class Target : MonoBehaviour {

		public GridTarget gridTarget;
		public TimelineTarget timelineTarget;

		public TargetIcon icon;


		private void Start() {
			//icon.SetSizes();
		}


		public void Select() {
			icon.EnableSelected(gridTarget.behavior);
		}

		public void Deselect() {
			icon.DisableSelected();
		}


		public void SetHandType(TargetHandType handType) {
			gridTarget.handType = handType;

			gridTarget.icon.SetHandType(handType);
			timelineTarget.icon.SetHandType(handType);
		}

		public void SetBehavior(TargetBehavior behavior) {
			gridTarget.behavior = behavior;
			gridTarget.icon.SetBehavior(behavior);
			timelineTarget.icon.SetBehavior(behavior);
		}

		public void SetBeatLength(float beatLength) {
			gridTarget.beatLength = beatLength;

			if (gridTarget.behavior == TargetBehavior.Hold)
				timelineTarget.icon.SetSustainLength(beatLength);
		}

		public void SetVelocity(TargetVelocity velocity) {
			gridTarget.velocity = velocity;
		}

	}
}