using UnityEngine;

namespace NotReaper.Targets {
	public class TargetGridMoveIntent {
		public TargetGridMoveIntent() {}
		
		public TargetGridMoveIntent(TargetGridMoveIntent other) {
			target = new TargetData(other.target);
			startingPosition = other.startingPosition;
			intendedPosition = other.intendedPosition;
		}

		public TargetData target;
		public Vector2 startingPosition;
		public Vector2 intendedPosition;
	}

	public class TargetTimelineMoveIntent {
		public TargetTimelineMoveIntent() {}

		public TargetTimelineMoveIntent(TargetTimelineMoveIntent other) {
			target = new TargetData(other.target);
			startTime = other.startTime;
			intendedTime = other.intendedTime;
		}
		public TargetData target;
		public float startTime;
		public float intendedTime;
	}
}