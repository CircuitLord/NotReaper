using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Targets {

	public class GridTarget : MonoBehaviour {

		public TargetIcon targetIcon;

		public TargetHandType handType;
		public TargetBehavior behavior;
		public float beatLength;
		public TargetVelocity velocity;

		public List<Target> chainedNotes;

	}
}