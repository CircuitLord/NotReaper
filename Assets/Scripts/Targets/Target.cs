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

		public Vector3 gridTargetPos;
		//public Vector3 timelineTargetPos;


		//Events and stuff:
		public event Action<Target, bool, bool> DeleteNoteEvent;
		public void DeleteNote(bool genUndoAction = true) {
			DeleteNoteEvent(this, genUndoAction, true);
		}

		public event Action<Target> TargetEnterLoadedNotesEvent;
		public void TargetEnterLoadedNotes() {
			TargetEnterLoadedNotesEvent(this);
		}

		public event Action<Target> TargetExitLoadedNotesEvent;
		public void TargetExitLoadedNotes() {
			TargetExitLoadedNotesEvent(this);
		}

		public event Action<Target> TargetSelectEvent;
		public void MakeTimelineSelectTarget() {
			TargetSelectEvent(this);
		}
		public event Action<Target, bool> TargetDeselectEvent;
		public void MakeTimelineDeselectTarget() {
			TargetDeselectEvent(this, false);
		}

		//I'm so good at naming stuff.
		public event Action<Target, bool> MakeTimelineUpdateSustainLengthEvent;
		public void MakeTimelineUpdateSustainLength(bool increase) {
			MakeTimelineUpdateSustainLengthEvent(this, increase);
		}


		//Do some stuff after all the target's references have been filled in by the timeline.
		public void Init() {
			gridTargetIcon.OnTryRemoveEvent += DeleteNote;
			timelineTargetIcon.OnTryRemoveEvent += DeleteNote;

			gridTargetIcon.IconEnterLoadedNotesEvent += TargetEnterLoadedNotes;
			gridTargetIcon.IconExitLoadedNotesEvent += TargetExitLoadedNotes;

			timelineTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;
			gridTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;

			timelineTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;
			gridTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;

			SetOutlineColor(NRSettings.config.selectedHighlightColor);

			if (behavior == TargetBehavior.Hold) {
				var gridHoldTargetManager = gridTargetIcon.GetComponentInChildren<HoldTargetManager>();

				gridHoldTargetManager.sustainLength = beatLength;
				gridHoldTargetManager.LoadSustainController();

				gridHoldTargetManager.OnTryChangeSustainEvent += MakeTimelineUpdateSustainLength;
			}

		
			gridTargetPos = gridTargetIcon.transform.localPosition;

		}

		public void SetOutlineColor(Color color) {
			timelineTargetIcon.SetOutlineColor(color);
			gridTargetIcon.SetOutlineColor(color);
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

		public void UpdateSustainBeatLength(float newBeatLength) {
			if (behavior != TargetBehavior.Hold) return;
			SetBeatLength(newBeatLength);
			//Updates the grid icon target sustain length.
			gridTargetIcon.GetComponentInChildren<HoldTargetManager>().UpdateSustainLength(newBeatLength);
		}

		public void SetVelocity(TargetVelocity newVel) {
			velocity = newVel;
			gridTargetIcon.velocity = velocity;
			timelineTargetIcon.velocity = velocity;

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