using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;

namespace NotReaper.Targets {


    public class GridTarget : Target {
        public TargetHandType handType;
        public TargetBehavior behavior;
        public float beatLength = 1;
        public TargetVelocity velocity;

        public List<GridTarget> chainedNotes;
    }
}