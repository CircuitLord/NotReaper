using UnityEngine;
using NotReaper.Timing;

namespace NotReaper.Targets {
	public class TargetGridMoveIntent {
		public TargetGridMoveIntent() {}
		
		public TargetGridMoveIntent(TargetGridMoveIntent other) {
			target = other.target;
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
			target = other.target;
			startTick = other.startTick;
			intendedTick = other.intendedTick;
		}
		public TargetData target;
		public QNT_Timestamp startTick;
		public QNT_Timestamp intendedTick;
	}
}