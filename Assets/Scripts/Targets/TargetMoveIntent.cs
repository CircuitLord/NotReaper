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
            
            targetData = other.targetData;
            startSiblingsToBeDestroyed = other.startSiblingsToBeDestroyed;
            startSiblingsToBeMoved = other.startSiblingsToBeMoved;

            endRepeaterSiblingsToBeCreated = other.endRepeaterSiblingsToBeCreated;
		}

        //These are the only data needed to be filled out. The rest will be calculated by `MoveTimelineTargets`
        public TargetData targetData;
		public QNT_Timestamp startTick;
		public QNT_Timestamp intendedTick;

        //Used to destroy targets in other repeater zones when a target moves out of its zone
        public List<TargetData> startSiblingsToBeDestroyed = new List<TargetData>();
        public List<TargetData> startSiblingsToBeMoved = new List<TargetData>();

        //List of targets that will be created when this action executes
        //Used when a target is moving into a repeater zone, and targets need to be created in the other zones
        public List<TargetData> endRepeaterSiblingsToBeCreated = new List<TargetData>();
    }
}