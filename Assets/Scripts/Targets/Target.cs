using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Targets {

	//Stores references to a timelineTarget and it's corrosponding gridTarget
	public class Target {

		public TargetHandType handType;
		public TargetBehavior behavior;
		public float beatLength;
		public TargetVelocity velocity;

		public List<Target> chainedNotes;


		public GridTarget gridTarget;
		public TimelineTarget timelineTarget;

		public void SetHandType(TargetHandType handType) {
			this.handType = handType;
			gridTarget.targetIcon.SetHandType(handType);
			timelineTarget.targetIcon.SetHandType(handType);
		}

		public void SetBehavior(TargetBehavior behavior) {
			this.behavior = behavior;
			gridTarget.targetIcon.SetBehavior(behavior);
			//timelineTarget.icon.SetBehavior(behavior);
		}

		public void SetBeatLength(float beatLength) {
			this.beatLength = beatLength;

			if (this.behavior == TargetBehavior.Hold) { }
			//timelineTarget.icon.SetSustainLength(beatLength);
		}

		public void SetVelocity(TargetVelocity velocity) {
			this.velocity = velocity;
		}

	}
}