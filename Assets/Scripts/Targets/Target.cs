using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Targets {


    public class Target : MonoBehaviour {
        public TimelineTarget timelineTarget;
        public GridTarget gridTarget;

        public TargetIcon icon;

        internal void SetHandType(TargetHandType handType) {
            gridTarget.handType = handType;
            gridTarget.icon.SetHandType(handType);
            timelineTarget.icon.SetHandType(handType);
        }

        internal void SetBehavior(TargetBehavior behavior) {
            gridTarget.behavior = behavior;
            timelineTarget.icon.SetBehavior(behavior);
            gridTarget.icon.SetBehavior(behavior);
        }

        internal void SetBeatLength(float beatLength) {
            gridTarget.beatLength = beatLength;

            if (gridTarget.behavior == TargetBehavior.Hold)
                timelineTarget.icon.SetSustainLength(beatLength);
        }

        public void SetVelocity(TargetVelocity velocity) {
            gridTarget.velocity = velocity;
        }

    }
}