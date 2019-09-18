using System;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Grid;

namespace NotReaper.Targets {

	public class TargetData {
		public float x; 
		public float y;
		public float beatTime;
		public float beatLength = 0.25f;
		public TargetVelocity velocity = TargetVelocity.Standard;
		public TargetHandType handType = TargetHandType.Left;
		public TargetBehavior behavior = TargetBehavior.Standard;

		public TargetData() {

		}

		public TargetData(Target target) {
			x = target.gridTargetIcon.transform.localPosition.x;
			y = target.gridTargetIcon.transform.localPosition.y;
			beatTime = target.gridTargetIcon.transform.localPosition.z;
			beatLength = target.beatLength;
			velocity = target.velocity;
			handType = target.handType;
			behavior = target.behavior;
		}

		public TargetData(Cue cue, float offset) {
			Vector2 pos = NotePosCalc.PitchToPos(cue);
			x = pos.x;
			y = pos.y;
			beatTime = (cue.tick - offset) / 480f;
			beatLength = cue.tickLength;
			velocity = cue.velocity;
			handType = cue.handType;
			behavior = cue.behavior;
		}
	};
}