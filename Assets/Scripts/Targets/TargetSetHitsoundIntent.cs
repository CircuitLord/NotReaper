using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Targets {
	public class TargetSetHitsoundIntent {
		public TargetSetHitsoundIntent() {}

		public TargetSetHitsoundIntent(TargetSetHitsoundIntent other) {
			target = new TargetData(other.target);
			startingVelocity = other.startingVelocity;
			newVelocity = other.newVelocity;
		}

		public TargetData target;
		public TargetVelocity startingVelocity;
		public TargetVelocity newVelocity;
	}
}