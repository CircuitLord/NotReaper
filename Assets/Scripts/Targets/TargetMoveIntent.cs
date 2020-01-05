using UnityEngine;
using NotReaper.Timing;
using System.Collections.Generic;

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
			startTick = other.startTick;
			intendedTick = other.intendedTick;

			startTargetData = other.startTargetData;
			startRepeaterSiblings = other.startRepeaterSiblings;
			
			endTargetData = other.endTargetData;
			endRepeaterSiblings = other.endRepeaterSiblings;
		}

		public TargetData startTargetData;
		public List<TargetData> startRepeaterSiblings = new List<TargetData>();
		public QNT_Timestamp startTick;
		
		public TargetData endTargetData;
		public List<TargetData> endRepeaterSiblings = new List<TargetData>();
		public QNT_Timestamp intendedTick;
	}
}