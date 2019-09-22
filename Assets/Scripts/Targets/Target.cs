using System;
using UnityEngine;
using NotReaper.Models;

namespace NotReaper.Targets {


	public class Target {

		private TargetIcon gridTargetIcon;
		private TargetIcon timelineTargetIcon;

		public TargetData data;

		//Events and stuff:
		public event Action<Target> DeleteNoteEvent;
		public void DeleteNote() {
			DeleteNoteEvent(this);
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


		public Target(TargetData targetData, TargetIcon timelineIcon, TargetIcon gridIcon) {
			timelineTargetIcon = timelineIcon;
			gridTargetIcon = gridIcon;

			data = new TargetData();
			data.PositionChangeEvent += OnGridPositionChanged;
			data.HandTypeChangeEvent += OnHandTypeChanged;
			data.BeatTimeChangeEvent += OnBeatTimeChanged;
			data.BeatLengthChangeEvent += OnBeatLengthChanged;

			timelineTargetIcon.Init(data);
			gridTargetIcon.Init(data);

			data.Copy(targetData);
			UpdateTimelineSustainLength();
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

			if (data.behavior == TargetBehavior.Hold) {
				var gridHoldTargetManager = gridTargetIcon.GetComponentInChildren<HoldTargetManager>();

				gridHoldTargetManager.sustainLength = data.beatLength;
				gridHoldTargetManager.LoadSustainController();

				gridHoldTargetManager.OnTryChangeSustainEvent += MakeTimelineUpdateSustainLength;
			}
		}

		public void Destroy() {
			if(gridTargetIcon) {
				UnityEngine.Object.Destroy(gridTargetIcon.gameObject);
			}
			if(timelineTargetIcon) {
				UnityEngine.Object.Destroy(timelineTargetIcon.gameObject);
			}

			data.PositionChangeEvent -= OnGridPositionChanged;
			data.HandTypeChangeEvent -= OnHandTypeChanged;
			data.BeatTimeChangeEvent -= OnBeatTimeChanged;
			data.BeatLengthChangeEvent -= OnBeatLengthChanged;
		}

		public float GetRelativeBeatTime() {
			return gridTargetIcon.transform.position.z;
		}

		public ParticleSystem GetHoldParticles() {
			return gridTargetIcon.holdParticles;
		}

		public void EnableSustainButtons() {
			gridTargetIcon.GetComponentInChildren<HoldTargetManager>().EnableSustainButtons();
		}

		public void DisableSustainButtons() {
			gridTargetIcon.GetComponentInChildren<HoldTargetManager>().DisableSustainButtons();
		}

		public void EnableColliders() {
			gridTargetIcon.sphereCollider.enabled = true;
			timelineTargetIcon.sphereCollider.enabled = true;
		}

		public void DisableColliders() {
			gridTargetIcon.sphereCollider.enabled = false;
			timelineTargetIcon.sphereCollider.enabled = false;
		}

		public void EnableGridColliders() {
			gridTargetIcon.sphereCollider.enabled = true;
		}

		public void DisableGridColliders() {
			gridTargetIcon.sphereCollider.enabled = false;
		}

		private void OnGridPositionChanged(float x, float y) {
			var pos = gridTargetIcon.transform.localPosition;
			pos.x = x;
			pos.y = y;
			gridTargetIcon.transform.localPosition = pos;

			if (data.behavior == TargetBehavior.Hold) {
				var holdEnd = gridTargetIcon.GetComponentInChildren<HoldTargetManager>().endMarker;
				if (holdEnd) holdEnd.transform.localPosition = new Vector3 (x, y, holdEnd.transform.localPosition.z);
			}
		}

		public void SetOutlineColor(Color color) {
			timelineTargetIcon.SetOutlineColor(color);
			gridTargetIcon.SetOutlineColor(color);
		}

		//Wrapper function for setting the hand types of both targets.
		private void OnHandTypeChanged(TargetHandType newType) {
			 //Handedness changes how the note is visually displayed in the timeline
			float xOffset = timelineTargetIcon.transform.localPosition.x;
			float yOffset = 0;
			float zOffset = 0;

			switch (data.handType) {
				case TargetHandType.Left:
					yOffset = 0.1f;
					zOffset = 0.1f;
					break;
				case TargetHandType.Right:
					yOffset = -0.1f;
					zOffset = 0.2f;
					break;
			}

			timelineTargetIcon.transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
		}

		private void OnBeatTimeChanged(float newTime) {
			var pos = gridTargetIcon.transform.localPosition;
			pos.z = newTime;
			gridTargetIcon.transform.localPosition = pos;

			var timelinePos = timelineTargetIcon.transform.localPosition;
			timelinePos.x = newTime;
			timelineTargetIcon.transform.localPosition = timelinePos;
		}

		private void OnBeatLengthChanged(float newBeatLength) {
			if (data.behavior != TargetBehavior.Hold) return;
			gridTargetIcon.GetComponentInChildren<HoldTargetManager>().UpdateSustainLength(newBeatLength);
		}

		public void UpdateTimelineSustainLength() {
			if (data.behavior != TargetBehavior.Hold) return;
			timelineTargetIcon.UpdateTimelineSustainLength();
		}

		public void Select() {
			timelineTargetIcon.EnableSelected(data.behavior);
			gridTargetIcon.EnableSelected(data.behavior);
		}

		public void Deselect() {
			timelineTargetIcon.DisableSelected();
			gridTargetIcon.DisableSelected();
		}
	}


}