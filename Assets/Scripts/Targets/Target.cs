using System;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Tools.ChainBuilder;

namespace NotReaper.Targets {


	public class Target {

		private TargetIcon gridTargetIcon;
		private TargetIcon timelineTargetIcon;

		public TargetData data;
		public bool transient;

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
			if(!transient) {
				TargetSelectEvent(this);
			}
		}
		public event Action<Target, bool> TargetDeselectEvent;
		public void MakeTimelineDeselectTarget() {
			if(!transient) {
				TargetDeselectEvent(this, false);
			}
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

			timelineTargetIcon.Init(this, data);
			gridTargetIcon.Init(this, data);

			//Must be after the two init's, unfortunate timing restiction, but the new objects must be active to find the hold target managers
			data.BehaviourChangeEvent += OnBehaviorChanged;

			data.Copy(targetData);
			UpdateTimelineSustainLength();
		}

		//Do some stuff after all the target's references have been filled in by the timeline.
		public void Init(Timeline timeline) {
			gridTargetIcon.OnTryRemoveEvent += DeleteNote;
			timelineTargetIcon.OnTryRemoveEvent += DeleteNote;

			gridTargetIcon.IconEnterLoadedNotesEvent += TargetEnterLoadedNotes;
			gridTargetIcon.IconExitLoadedNotesEvent += TargetExitLoadedNotes;

			timelineTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;
			gridTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;

			timelineTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;
			gridTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;

			SetOutlineColor(NRSettings.config.selectedHighlightColor);

			if(transient) {
				foreach (Renderer r in gridTargetIcon.GetComponentsInChildren<Renderer>(true)) {
					if (r.name == "WhiteRing") {
						var color = r.material.GetColor("_Tint");
						color.r = 0.1f;
						color.g = 0.1f;
						color.b = 0.1f;
						r.material.SetColor("_Tint", color);
					}
				}
				foreach (Renderer r in timelineTargetIcon.GetComponentsInChildren<SpriteRenderer>(true)) {
					var color = r.material.color;
					color.r = 0.5f;
					color.g = 0.5f;
					color.b = 0.5f;
					r.material.color = color;
				}
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
			data.BehaviourChangeEvent -= OnBehaviorChanged;
		}

		public float GetRelativeBeatTime() {
			return gridTargetIcon.transform.position.z;
		}

		public ParticleSystem GetHoldParticles() {
			return gridTargetIcon.holdParticles;
		}

		public void EnableSustainButtons() {
			gridTargetIcon.sustainButtons.SetActive(true);
		}

		public void DisableSustainButtons() {
			gridTargetIcon.sustainButtons.SetActive(false);
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

			if(data.behavior == TargetBehavior.NR_Pathbuilder && data.pathBuilderData.generatedNotes.Count > 0) {
				var firstNote = data.pathBuilderData.generatedNotes[0];
				var delta = firstNote.position - new Vector2(x, y);

				foreach(TargetData note in data.pathBuilderData.generatedNotes) {
					note.position -= delta;
				}
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

			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				foreach(TargetData note in data.pathBuilderData.generatedNotes) {
					note.handType = newType;
				}
			}
		}

		private void OnBeatTimeChanged(float newTime) {
			var pos = gridTargetIcon.transform.localPosition;
			pos.z = newTime;
			gridTargetIcon.transform.localPosition = pos;

			var timelinePos = timelineTargetIcon.transform.localPosition;
			timelinePos.x = newTime;
			timelineTargetIcon.transform.localPosition = timelinePos;

			if(data.behavior == TargetBehavior.NR_Pathbuilder && data.pathBuilderData.generatedNotes.Count > 0) {
				var firstNote = data.pathBuilderData.generatedNotes[0];
				var delta = firstNote.beatTime - newTime;

				foreach(TargetData note in data.pathBuilderData.generatedNotes) {
					note.beatTime -= delta;
				}
			}
		}

		private void OnBeatLengthChanged(float newBeatLength) {
			if (!data.supportsBeatLength) return;
			
			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				ChainBuilder.CalculateChainNotes(data);
			}

			gridTargetIcon.GetComponentInChildren<HoldTargetManager>().UpdateSustainLength(newBeatLength);
		}

		private void OnBehaviorChanged(TargetBehavior behavior) {
			if (data.supportsBeatLength) {
				var gridHoldTargetManager = gridTargetIcon.GetComponentInChildren<HoldTargetManager>();

				gridHoldTargetManager.sustainLength = data.beatLength;
				gridHoldTargetManager.LoadSustainController();

				gridHoldTargetManager.OnTryChangeSustainEvent += MakeTimelineUpdateSustainLength;

				if(data.beatLength < 480) {
					data.beatLength = 480;
				}
			}
		}

		public void UpdateTimelineSustainLength() {
			if (!data.supportsBeatLength) return;
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

		public void UpdatePath() {
			gridTargetIcon.UpdatePath();
		}
	}


}