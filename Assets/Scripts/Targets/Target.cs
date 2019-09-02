using System;
using UnityEngine;
using NotReaper.Models;

namespace NotReaper.Targets {


	public class Target {

		public TargetIcon gridTargetIcon;
		public TargetIcon timelineTargetIcon;

		public TargetHandType handType = TargetHandType.Left;
		public TargetBehavior behavior = TargetBehavior.Standard;
		public TargetVelocity velocity = TargetVelocity.Standard;
		public float beatLength = 0.25f;
		public bool isSelected = false;


		//Events and stuff:
		public event Action<Target, bool> DeleteNoteEvent;
		public void DeleteNote(bool genUndoAction = true) {
			DeleteNoteEvent(this, genUndoAction);
		}


		public void Init() {
			gridTargetIcon.OnTryRemoveEvent += DeleteNote;
		}

		//Wrapper function for setting the hand types of both targets.
		public void SetHandType(TargetHandType newType) {
			gridTargetIcon.SetHandType(newType);
			timelineTargetIcon.SetHandType(newType);
			handType = newType;
		}


		public void SetBehavior(TargetBehavior newBehavior) {
			gridTargetIcon.SetBehavior(newBehavior);
			timelineTargetIcon.SetBehavior(newBehavior);
			behavior = newBehavior;
		}

		public void SetBeatLength(float newBeatLength) {
			beatLength = newBeatLength;

			if (behavior == TargetBehavior.Hold) {
				timelineTargetIcon.SetSustainLength(newBeatLength);
			}
		}

		public void SetVelocity(TargetVelocity newVel) {
			velocity = newVel;
		}

		public void Select() {
			timelineTargetIcon.EnableSelected(behavior);
			gridTargetIcon.EnableSelected(behavior);
			isSelected = true;
		}

		public void Deselect() {
			timelineTargetIcon.DisableSelected();
			gridTargetIcon.DisableSelected();
			isSelected = false;
		}




	}


}