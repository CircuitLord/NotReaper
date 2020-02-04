using System;
using System.Collections;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Tools.ChainBuilder;
using NotReaper.Timing;
using DG.Tweening;
using System.Collections.Generic;

namespace NotReaper.Targets {


	public class Target {

		private TargetIcon gridTargetIcon;
		private TargetIcon timelineTargetIcon;

		private bool noteIsAnimating = false;

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


		public Target(TargetData targetData, TargetIcon timelineIcon, TargetIcon gridIcon, bool transient) {
			timelineTargetIcon = timelineIcon;
			gridTargetIcon = gridIcon;

			data = targetData;
			data.PositionChangeEvent += OnGridPositionChanged;
			data.HandTypeChangeEvent += OnHandTypeChanged;
			data.TickChangeEvent += OnTickChanged;
			data.BeatLengthChangeEvent += OnBeatLengthChanged;

			timelineTargetIcon.Init(this, data);
			gridTargetIcon.Init(this, data);

			//Must be after the two init's, unfortunate timing restiction, but the new objects must be active to find the hold target managers
			data.BehaviourChangeEvent += OnBehaviorChanged;
			
			UpdateTimelineSustainLength();

			gridTargetIcon.OnTryRemoveEvent += DeleteNote;
			timelineTargetIcon.OnTryRemoveEvent += DeleteNote;

			gridTargetIcon.IconEnterLoadedNotesEvent += TargetEnterLoadedNotes;
			gridTargetIcon.IconExitLoadedNotesEvent += TargetExitLoadedNotes;

			timelineTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;
			gridTargetIcon.TrySelectEvent += MakeTimelineSelectTarget;

			timelineTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;
			gridTargetIcon.TryDeselectEvent += MakeTimelineDeselectTarget;

			SetOutlineColor(NRSettings.config.selectedHighlightColor);

			this.transient = transient;
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

			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				data.pathBuilderData.InitialAngleChangedEvent += UpdatePathInitialAngle;
				data.pathBuilderData.RecalculateEvent += RecalculatePathbuilderData;
				data.pathBuilderData.RecalculateFinishedEvent += UpdatePath;

				UpdatePathInitialAngle();
			}
		}

		public void Destroy(Timeline timeline) {
			if(gridTargetIcon) {
				UnityEngine.Object.Destroy(gridTargetIcon.gameObject);
			}
			if(timelineTargetIcon) {
				UnityEngine.Object.Destroy(timelineTargetIcon.gameObject);
			}

			data.PositionChangeEvent -= OnGridPositionChanged;
			data.HandTypeChangeEvent -= OnHandTypeChanged;
			data.TickChangeEvent -= OnTickChanged;
			data.BeatLengthChangeEvent -= OnBeatLengthChanged;
			data.BehaviourChangeEvent -= OnBehaviorChanged;

			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				data.pathBuilderData.InitialAngleChangedEvent -= UpdatePathInitialAngle;
				data.pathBuilderData.RecalculateEvent -= RecalculatePathbuilderData;
				data.pathBuilderData.RecalculateFinishedEvent -= UpdatePath;

				data.pathBuilderData.DeleteCreatedNotes(timeline);
			}
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

				gridTargetIcon.UpdatePath();
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

		private void OnTickChanged(QNT_Timestamp newTime) {
			var pos = gridTargetIcon.transform.localPosition;
			pos.z = newTime.ToBeatTime();
			gridTargetIcon.transform.localPosition = pos;

			var timelinePos = timelineTargetIcon.transform.localPosition;
			timelinePos.x = newTime.ToBeatTime();
			timelineTargetIcon.transform.localPosition = timelinePos;

			if(data.behavior == TargetBehavior.NR_Pathbuilder && data.pathBuilderData.generatedNotes.Count > 0) {
				var firstNote = data.pathBuilderData.generatedNotes[0];
				var delta = firstNote.time - newTime;

				foreach(TargetData note in data.pathBuilderData.generatedNotes) {
					note.time -= delta;
				}
			}
		}

		private void OnBeatLengthChanged(QNT_Duration newBeatLength) {
			if (!data.supportsBeatLength) return;
			
			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				ChainBuilder.GenerateChainNotes(data);
			}
		}

		private void OnBehaviorChanged(TargetBehavior oldBehavior, TargetBehavior newBehavior) {
			if (data.supportsBeatLength) {
				var gridHoldTargetManager = gridTargetIcon.GetComponentInChildren<HoldTargetManager>();

				gridHoldTargetManager.sustainLength = data.beatLength;
				gridHoldTargetManager.LoadSustainController();

				gridHoldTargetManager.OnTryChangeSustainEvent += MakeTimelineUpdateSustainLength;

				gridTargetIcon.UpdatePath();
			}
			else {
				DisableSustainButtons();
			}

			if(data.behavior == TargetBehavior.NR_Pathbuilder) {
				data.pathBuilderData.InitialAngleChangedEvent += UpdatePathInitialAngle;
				data.pathBuilderData.RecalculateEvent += RecalculatePathbuilderData;
				data.pathBuilderData.RecalculateFinishedEvent += UpdatePath;
			}

			if(oldBehavior == TargetBehavior.NR_Pathbuilder) {
				data.pathBuilderData.InitialAngleChangedEvent -= UpdatePathInitialAngle;
				data.pathBuilderData.RecalculateEvent -= RecalculatePathbuilderData;
				data.pathBuilderData.RecalculateFinishedEvent -= UpdatePath;
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

		public void RecalculatePathbuilderData() {
			if(data.behavior != TargetBehavior.NR_Pathbuilder) return;
			ChainBuilder.CalculateChainNotes(data);
		}

		public void UpdatePathInitialAngle() {
			if(data.behavior != TargetBehavior.NR_Pathbuilder) return;

			gridTargetIcon.UpdatePathInitialAngle(data.pathBuilderData.initialAngle);
		}

		public void OnNoteHit() {

			if (noteIsAnimating) return;
			noteIsAnimating = true;


			if (data.behavior == TargetBehavior.Hold) {
				Timeline.instance.StartCoroutine(AnimateHoldSpin());
			}
			else {
				
				Timeline.instance.StartCoroutine(AnimateNoteBounce());
			}
			

		}

		private IEnumerator AnimateHoldSpin() {

			if (Timeline.instance.paused) yield break;
			
			noteIsAnimating = true;


			float time = (float)(data.beatLength.tick * (60 / (Timeline.instance.GetBpmFromTime(data.time) * 480)));

			time /= Timeline.instance.playbackSpeed;

			//float time = (float)(data.beatLength.tick / (480f * Timeline.instance.GetBpmFromTime(data.time)));
			
			float extensionTime = (float)(745 * (60 / (Timeline.instance.GetBpmFromTime(data.time + data.beatLength) * 480)));

			extensionTime /= Timeline.instance.playbackSpeed;


			gridTargetIcon.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 1080), time + extensionTime).SetRelative().SetEase(Ease.InSine);
			gridTargetIcon.transform.DOScale(0.8f, time + extensionTime).SetEase(Ease.Linear);

			gridTargetIcon.holdEndTrans.DOLocalRotate(new Vector3(0.0f, 0.0f, 1080), time + extensionTime).SetRelative().SetEase(Ease.InSine);
			gridTargetIcon.holdEndTrans.DOScale(0.8f, time + extensionTime).SetEase(Ease.Linear);
			
			yield return new WaitForSeconds(time + extensionTime);

			if (gridTargetIcon != null) {
				gridTargetIcon.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutCubic);
				gridTargetIcon.holdEndTrans.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutCubic);
			}


			noteIsAnimating = false;
			yield break;
			//yield return new WaitForSeconds();
		}

		private IEnumerator AnimateNoteBounce() {
			DOTween.To((float scale) => {
				gridTargetIcon.transform.localScale = new Vector3(scale, scale, 1f);
			}, NRSettings.config.noteHitScale, 1f, 0.3f).SetEase(Ease.OutCubic);
			
			yield return new WaitForSeconds(0.3f);
			noteIsAnimating = false;
		}

		public void AddTargetIconsCloseToPointAtTime(List<TargetIcon> icons, QNT_Timestamp time, Vector2 point) {
			if(gridTargetIcon.IsInValidTime(time) && gridTargetIcon.IsCloseToPoint(point)) {
				icons.Add(gridTargetIcon);
			}

			if(timelineTargetIcon.IsInValidTime(time) && timelineTargetIcon.IsCloseToPoint(point)) {
				icons.Add(timelineTargetIcon);
			}
		}

		public bool IsInsideRectAtTime(QNT_Timestamp time, Rect rect) {
			return (gridTargetIcon.IsInValidTime(time) && gridTargetIcon.IsInsideRect(rect)) || 
				(timelineTargetIcon.IsInValidTime(time) && timelineTargetIcon.IsInsideRect(rect));
		}

		public bool IsTimelineInsideRect(Rect rect) {
			return timelineTargetIcon.IsInsideRect(rect);
		}
	}
}