using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using NAudio.Midi;
using NotReaper.Grid;
using NotReaper.IO;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.Tools;
using NotReaper.Tools.ChainBuilder;
using NotReaper.UI;
using NotReaper.UserInput;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Application = UnityEngine.Application;
using NotReaper.Timing;

using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace NotReaper {

	public class NoteEnumerator : IEnumerable<Target> {
		public NoteEnumerator(QNT_Timestamp start, QNT_Timestamp end) {
			this.start = start;
			this.end = end;
		}

		public QNT_Timestamp start;
		public QNT_Timestamp end;

		public bool startInclusive = true;
		public bool endInclusive = true;
		public bool reverse = false;

		public IEnumerator<Target> GetEnumerator() {
			if(Timeline.orderedNotes.Count == 0) {
				yield break;
			}

			var result = Timeline.BinarySearchOrderedNotes(start);
			int index = result.index;

			//Invalid index? No iteration
			if(index >= Timeline.orderedNotes.Count) {
				yield break;
			}

			//We didn't find an exact result, so search for the nearest
			if(!result.found) {

				//Go back until we find a note with a time less then the start
				while(index >= 0) {
					QNT_Timestamp time = Timeline.orderedNotes[index].data.time;
					if(time < start) {
						break;
					}

					--index;
				}

				if(index < 0) {
					index = 0;
				}

				//Increment up to and including start
				while(Timeline.orderedNotes[index].data.time < start) {
					++index;
				}
			}

			//Invalid index? No iteration
			if(index >= Timeline.orderedNotes.Count) {
				yield break;
			}

			//If we are not inclusive to starting time, then move up until we get a time after the start
			if(!startInclusive) {
				while(Timeline.orderedNotes[index].data.time <= start) {
					++index;
				}
			}

			//Iterate over the valid notes
			if(!reverse) {
				for(int i = index; i < Timeline.orderedNotes.Count; ++i) {
					if(Timeline.orderedNotes[i].data.time > end) {
						yield break;
					}
					
					if(!endInclusive && Timeline.orderedNotes[i].data.time == end) {
						yield break;
					}

					yield return Timeline.orderedNotes[i];
				}
			}
			else {
				int endIndex = index;

				while(endIndex < Timeline.orderedNotes.Count && Timeline.orderedNotes[endIndex].data.time < end) {
					++endIndex;
				}

				if(endInclusive) {
					while(endIndex < Timeline.orderedNotes.Count && Timeline.orderedNotes[endIndex].data.time <= end) {
						++endIndex;
					}

					--endIndex;
				}

				if(endIndex >= Timeline.orderedNotes.Count) {
					endIndex = Timeline.orderedNotes.Count - 1;
				}

				for(int i = endIndex; i >= index; --i) {
					yield return Timeline.orderedNotes[i];
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}


	public class Timeline : MonoBehaviour {

		public static Timeline instance;
		
		//Hidden public values
		[HideInInspector] public static AudicaFile audicaFile;

		[HideInInspector] public static SongDesc desc;

		[Header("Audio Stuff")]
		[SerializeField] private Transform spectrogram;

		[Header("UI Elements")]
		[SerializeField] private MiniTimeline miniTimeline;
		[SerializeField] private TextMeshProUGUI songTimestamp;
		[SerializeField] private TextMeshProUGUI curTick;
		[SerializeField] private TextMeshProUGUI curDiffText;

		[SerializeField] private HorizontalSelector beatSnapSelector;

		[Header("Prefabs")]
		public TargetIcon timelineTargetIconPrefab;
		public TargetIcon gridTargetIconPrefab;
		public GameObject BPM_MarkerPrefab;

		[Header("Extras")]
		[SerializeField] private NRDiscordPresence nrDiscordPresence;
		[SerializeField] private DifficultyManager difficultyManager;
		[SerializeField] public EditorToolkit Tools;
		[SerializeField] public Transform timelineTransformParent;
		[SerializeField] private Transform gridTransformParent;
		public static Transform gridNotesStatic;
		public static Transform timelineNotesStatic;
		[SerializeField] private Renderer timelineBG;
		[SerializeField] private TextMeshProUGUI beatSnapWarningText;
		
		
		public Slider musicVolumeSlider;
		public Slider hitSoundVolumeSlider;

		[Header("Configuration")]
		public float playbackSpeed = 1f;

		public float musicVolume = 0.5f; 
		public float sustainVolume = 0.5f;
		public float previewDuration = 0.1f;

		//Target Lists
		public List<Target> notes;
		public static List<Target> orderedNotes;
		public List<Target> selectedNotes;

		public static List<Target> loadedNotes;

		public static bool inTimingMode = false;
		public static bool audioLoaded = false;
		public static bool audicaLoaded = false;

		private Color leftColor;
		private Color rightColor;
		private Color bothColor;
		private Color neitherColor;

		private static readonly int MainTex = Shader.PropertyToID("_MainTex");
		
		/// <summary>
		/// The current time in the song
		/// </summary>
		/// <value></value>
		public static QNT_Timestamp time { get; set; }

		public int beatSnap { get; private set; } = 4;

		[HideInInspector] public static int scale = 20;
		public static float scaleTransform;
		private float targetScale = 0.7f;
		private float scaleOffset = 0;
		public static Relative_QNT offset = new Relative_QNT(0);

		/// <summary>
		/// If the timeline is currently being moved by an animation.
		/// </summary>
		private bool animatingTimeline = false;

		[HideInInspector] public bool hover = false;
		public bool paused = true;

		public Button generateAudicaButton;
		public Button loadAudioFileTiming;

		public List<TempoChange> tempoChanges = new List<TempoChange>();
		private List<GameObject> bpmMarkerObjects = new List<GameObject>();

		[SerializeField] private PrecisePlayback songPlayback;

		[SerializeField]
		private AudioWaveformVisualizer waveformVisualizer;
		
		public bool areNotesSelected => selectedNotes.Count > 0;


		[SerializeField] public LineRenderer leftHandTraceLine; 
		[SerializeField] public LineRenderer rightHandTraceLine; 
		[SerializeField] public GameObject dualNoteTraceLinePrefab; 

		List<LineRenderer> dualNoteTraceLines = new List<LineRenderer>();

		//Tools
		private void Start() {

			instance = this;
			
			//Load the config file
			NRSettings.LoadSettingsJson();
			
			//Initialize autoupdating:
			HandleAutoupdater();


			notes = new List<Target>();
			orderedNotes = new List<Target>();
			loadedNotes = new List<Target>();
			selectedNotes = new List<Target>();
			
			gridNotesStatic = gridTransformParent;
			timelineNotesStatic = timelineTransformParent;

			Physics.autoSyncTransforms = false;

			ChainBuilder.timeline = this;

			musicVolumeSlider.onValueChanged.AddListener(val => {
				musicVolume = val;
				NRSettings.config.mainVol = musicVolume;
				NRSettings.SaveSettingsJson();
			});

			hitSoundVolumeSlider.onValueChanged.AddListener(val => {
				NRSettings.config.noteVol = val;
				NRSettings.SaveSettingsJson();
			});

			NRSettings.OnLoad(() => {
				sustainVolume = NRSettings.config.sustainVol;
				musicVolume = NRSettings.config.mainVol;
				musicVolumeSlider.value = musicVolume;
				hitSoundVolumeSlider.value = NRSettings.config.noteVol;
				
				SetAudioDSP();

				if (NRSettings.config.clearCacheOnStartup) {
					HandleCache.ClearCache();
				}
			});


			beatSnapWarningText.DOFade(0f, 0f);
		}


		private void HandleAutoupdater() {


		}
		
		public void UpdateUIColors() {
			curDiffText.color = NRSettings.config.rightColor;
			leftColor = NRSettings.config.leftColor;
			rightColor = NRSettings.config.rightColor;
			bothColor = UserPrefsManager.bothColor;
			neitherColor = UserPrefsManager.neitherColor;
		}

		void OnApplicationQuit() {
			//DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "\\temp\\");
			//dir.Delete(true);
		}

		public void SortOrderedList() {
			orderedNotes.Sort((left, right) =>  left.data.time.CompareTo(right.data.time));
		}


		public struct BinarySearchResult {
			public bool found;
			public int index;
		}

		public static BinarySearchResult BinarySearchOrderedNotes(QNT_Timestamp cueTime)
		{ 
			BinarySearchResult result;

			int min = 0;
			int max = orderedNotes.Count - 1;
			while (min <= max) {
				int mid = (min + max) / 2;
				QNT_Timestamp midCueTime = orderedNotes[mid].data.time;
				if (cueTime == midCueTime) {
					while(mid != 0 && orderedNotes[mid - 1].data.time == cueTime) {
						--mid;
					}

					result.index = mid;
					result.found = true;
					return result;
				}
				else if (cueTime < midCueTime) {
					max = mid - 1;
				}
				else {
					min = mid + 1;
				}
			}

			result.index = min;
			result.found = false;
			return result;
		}

		public TargetData FindTargetData(QNT_Timestamp time, TargetBehavior behavior, TargetHandType handType) {
			BinarySearchResult res = BinarySearchOrderedNotes(time);
			if(res.found == false) {
				Debug.LogWarning("Couldn't find note with time " + time);
				return null;
			}

			for(int i = res.index; i < orderedNotes.Count; ++i) {
				Target t = orderedNotes[i];
				if (t.data.time == time &&
					t.data.behavior == behavior &&
					t.data.handType == handType) {
					return t.data;
				}
			}

			Debug.LogWarning("Couldn't find note with time " + time + " and index " + res.index);
			return null;
		}
		
		public Target FindNote(TargetData data) {
			BinarySearchResult res = BinarySearchOrderedNotes(data.time);
			if(res.found == false) {
				Debug.LogWarning("Couldn't find note with time " + data.time);
				return null;
			}

			for(int i = res.index; i < orderedNotes.Count; ++i) {
				Target t = orderedNotes[i];
				if (t.data.ID == data.ID) {
					return t;
				}
			}

			Debug.LogWarning("Couldn't find note with time " + data.time + " and index " + res.index);
			return null;
		}

		public List<Target> FindNotes(List<TargetData> targetDataList) {
			List<Target> foundNotes = new List<Target>();
			foreach (TargetData data in targetDataList) {
				foundNotes.Add(FindNote(data));
			}
			return foundNotes;
		}

		//When loading from cues, use this.
		public TargetData GetTargetDataForCue(Cue cue) {
			TargetData data = new TargetData(cue);
			if (data.time.tick == 0) data.time = new QNT_Timestamp(120);
			return data;
		}


		//Use when adding a singular target to the project (from the user)
		public void AddTarget(float x, float y) {
			if(audicaLoaded == false) {
				return;
			}

			TargetData data = new TargetData();
			data.x = x;
			data.y = y;
			data.handType = EditorInput.selectedHand;
			data.behavior = EditorInput.selectedBehavior;

			QNT_Timestamp tempTime = GetClosestBeatSnapped(time, (uint)beatSnap);

			foreach (Target target in loadedNotes) {
				if (target.data.time ==  tempTime && (target.data.handType == EditorInput.selectedHand) && (EditorInput.selectedTool != EditorTool.Melee) && (EditorInput.selectedTool != EditorTool.Mine)) return;
			}

			data.time = GetClosestBeatSnapped(time, (uint)beatSnap);

			//Default sustains length should be more than 0.
			if (data.supportsBeatLength) {
				data.beatLength = Constants.QuarterNoteDuration;
			} else {
				data.beatLength = Constants.SixteenthNoteDuration;
			}

			switch (EditorInput.selectedVelocity) {
				case UITargetVelocity.Standard:
					data.velocity = TargetVelocity.Standard;
					break;

				case UITargetVelocity.Snare:
					data.velocity = TargetVelocity.Snare;
					break;

				case UITargetVelocity.Percussion:
					data.velocity = TargetVelocity.Percussion;
					break;

				case UITargetVelocity.ChainStart:
					data.velocity = TargetVelocity.ChainStart;
					break;

				case UITargetVelocity.Chain:
					data.velocity = TargetVelocity.Chain;
					break;

				case UITargetVelocity.Melee:
					data.velocity = TargetVelocity.Melee;
					break;
				
				case UITargetVelocity.Mine:
					data.velocity = TargetVelocity.Mine;
					break;

				default:
					data.velocity = TargetVelocity.Standard;
					break;
			}

			var action = new NRActionAddNote {targetData = data};
			Tools.undoRedoManager.AddAction(action);

			songPlayback.PlayHitsound(time);
		}

		//Adds a target directly to the timeline. targetData is kept as a reference NOT copied
		public Target AddTargetFromAction(TargetData targetData, bool transient = false) {

			var timelineTargetIcon = Instantiate(timelineTargetIconPrefab, timelineTransformParent);
			timelineTargetIcon.location = TargetIconLocation.Timeline;
			var transform1 = timelineTargetIcon.transform;
			transform1.localPosition = new Vector3(targetData.time.ToBeatTime(), 0, 0);

			Vector3 noteScale = transform1.localScale;
			noteScale.x = targetScale;
			transform1.localScale = noteScale;

			var gridTargetIcon = Instantiate(gridTargetIconPrefab, gridTransformParent);
			gridTargetIcon.transform.localPosition = new Vector3(targetData.x, targetData.y, targetData.time.ToBeatTime());
			gridTargetIcon.location = TargetIconLocation.Grid;

			Target target = new Target(targetData, timelineTargetIcon, gridTargetIcon, transient);

			notes.Add(target);
			orderedNotes = notes.OrderBy(v => v.data.time.tick).ToList();

			//Subscribe to the delete note event so we can delete it if the user wants. And other events.
			target.DeleteNoteEvent += DeleteTarget;

			target.TargetEnterLoadedNotesEvent += AddLoadedNote;
			target.TargetExitLoadedNotesEvent += RemoveLoadedNote;

			target.TargetSelectEvent += SelectTarget;
			target.TargetDeselectEvent += DeselectTarget;

			target.MakeTimelineUpdateSustainLengthEvent += UpdateSustainLength;

			//Trigger all callbacks on the note
			targetData.Copy(targetData); 

			//Also generate chains if needed
			if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
				ChainBuilder.GenerateChainNotes(targetData);
			}

			return target;
		}

		public List<float> DetectBPM(QNT_Timestamp start, QNT_Timestamp end) {
			return BPM.Detect(songPlayback.song, this, start, end);
		}

        private void UpdateSustains() {
			foreach (var note in loadedNotes) {
				if (note.data.behavior == TargetBehavior.Hold) {
					if ((note.GetRelativeBeatTime() < 0) && (note.GetRelativeBeatTime() + note.data.beatLength.ToBeatTime() > 0))
					{

						var particles = note.GetHoldParticles();
						if (!particles.isEmitting) {
							particles.Play();

							float panPos = (float) (note.data.x / 7.15);
							if (audicaFile.usesLeftSustain && note.data.handType == TargetHandType.Left) {
								songPlayback.leftSustainVolume = sustainVolume;
								songPlayback.leftSustain.pan = panPos;
							} else if (audicaFile.usesRightSustain && note.data.handType == TargetHandType.Right) {
								songPlayback.rightSustainVolume = sustainVolume;
								songPlayback.rightSustain.pan = panPos;
							}

							var main = particles.main;
							main.startColor = note.data.handType == TargetHandType.Left ? new Color(leftColor.r, leftColor.g, leftColor.b, 1) : new Color(rightColor.r, rightColor.g, rightColor.b, 1);
						}

						ParticleSystem.Particle[] parts = new ParticleSystem.Particle[particles.particleCount];
						particles.GetParticles(parts);

						for (int i = 0; i < particles.particleCount; ++i) {
							parts[i].position = new Vector3(parts[i].position.x, parts[i].position.y, 0);
						}

						particles.SetParticles(parts, particles.particleCount);

					} else
					{
						var particles = note.GetHoldParticles();
						if (particles.isEmitting) {
							particles.Stop();
							if (note.data.handType == TargetHandType.Left) {
								songPlayback.leftSustainVolume = 0.0f;
							} else if (note.data.handType == TargetHandType.Right) {
								songPlayback.rightSustainVolume = 0.0f;
							}
						}
					}
				}
			}
		}


		public static void AddLoadedNote(Target target) {
			loadedNotes.Add(target);
		}

		public static void RemoveLoadedNote(Target target) {
			loadedNotes.Remove(target);
		}



		public void SelectTarget(Target target) {
			if (!selectedNotes.Contains(target)) {
				selectedNotes.Add(target);
				target.Select();
			}
		}


		public void DeselectTarget(Target target, bool resettingAll = false) {
			if (selectedNotes.Contains(target)) {

				target.Deselect();

				if (!resettingAll) {
					selectedNotes.Remove(target);
				}
			}

		}

		public void DeselectAllTargets() {
			if (!audicaLoaded) return;

			Camera.main.farClipPlane = 50;

			foreach (Target target in selectedNotes) {
				DeselectTarget(target, true);
			}

			selectedNotes = new List<Target>();
		}
		
		/// <summary>
		/// Updates a sustain length from the buttons next to sustains.
		/// </summary>
		/// <param name="target">The target to affect</param>
		/// <param name="increase">If true, increase by one beat snap, if false, the opposite.</param>
		public void UpdateSustainLength(Target target, bool increase) {
			if (!target.data.supportsBeatLength) return;

			QNT_Duration increment = Constants.DurationFromBeatSnap((uint)beatSnap);
			QNT_Duration minimum = Constants.SixteenthNoteDuration;
			
			if (increase) {
				if (target.data.beatLength < increment) target.data.beatLength = new QNT_Duration(0);
				target.data.beatLength += increment;
			} else {
				target.data.beatLength -= increment;
			}

			target.UpdatePath();
		}

		public void MoveGridTargets(List<TargetGridMoveIntent> intents) {
			var action = new NRActionGridMoveNotes();
			action.targetGridMoveIntents = intents.Select(intent => new TargetGridMoveIntent(intent)).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		public void MoveTimelineTargets(List<TargetTimelineMoveIntent> intents) {
			SortOrderedList();
			var action = new NRActionTimelineMoveNotes();
			action.targetTimelineMoveIntents = intents.Select(intent => new TargetTimelineMoveIntent(intent)).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		public void PasteCues(List<TargetData> cues, QNT_Timestamp pasteBeatTime) {

			// paste new targets in the original locations
			var targetDataList = cues.Select(copyData => {
				var data = new TargetData(copyData);

				if(data.behavior == TargetBehavior.NR_Pathbuilder) {
					data.pathBuilderData = new PathBuilderData();
					data.pathBuilderData.Copy(copyData.pathBuilderData);
				}

				return data;
			}).ToList();

			// find the soonest target in the selection
			QNT_Timestamp earliestTargetBeatTime = new QNT_Timestamp(long.MaxValue);
			foreach (TargetData data in targetDataList) {
				QNT_Timestamp time = data.time;
				if (time < earliestTargetBeatTime) {
					earliestTargetBeatTime = time;
				}
			}

			// shift all by the amount needed to move the earliest note to now
			Relative_QNT diff = pasteBeatTime - earliestTargetBeatTime;
			foreach (TargetData data in targetDataList) {
				data.time += diff;
			}

			var action = new NRActionMultiAddNote();
			action.affectedTargets = targetDataList;
			Tools.undoRedoManager.AddAction(action);

			DeselectAllTargets();
			FindNotes(targetDataList).ForEach(target => SelectTarget(target));
		}

		// Invert the selected targets' colour
		public void SwapTargets(List<Target> targets) {
			var action = new NRActionSwapNoteColors();
			action.affectedTargets = targets.Select(target => target.data).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		// Flip the selected targets on the grid about the X
		public void FlipTargetsHorizontal(List<Target> targets) {
			var action = new NRActionHFlipNotes();
			action.affectedTargets = targets.Select(target => target.data).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		// Flip the selected targets on the grid about the Y
		public void FlipTargetsVertical(List<Target> targets) {
			var action = new NRActionVFlipNotes();
			action.affectedTargets = targets.Select(target => target.data).ToList();
			Tools.undoRedoManager.AddAction(action);
		}
		
		public void SetTargetHitsounds(List<TargetSetHitsoundIntent> intents) {
			var action = new NRActionSetTargetHitsound();
			action.targetSetHitsoundIntents = intents.Select(intent => new TargetSetHitsoundIntent(intent)).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		public void SetTargetBehaviors(NRActionSetTargetBehavior action) {
			Tools.undoRedoManager.AddAction(action);
		}

		public void DeleteTarget(Target target) {
			var action = new NRActionRemoveNote();
			action.targetData = target.data;
			Tools.undoRedoManager.AddAction(action);
		}

		public void DeleteTargetFromAction(TargetData targetData) {
			Target target = FindNote(targetData);
			if (target == null) return;

			notes.Remove(target);
			orderedNotes.Remove(target);
			loadedNotes.Remove(target);
			selectedNotes.Remove(target);

			target.Destroy(this);
			target = null;
		}

		public void DeleteTargets(List<Target> targets) {
			var action = new NRActionMultiRemoveNote();
			action.affectedTargets = targets.Select(target => target.data).ToList();
			Tools.undoRedoManager.AddAction(action);
		}

		public void DeleteAllTargets() {
			var notesTemp = notes.ToList();
			foreach (Target target in notesTemp) {
				target.Destroy(this);
			}

			notes = new List<Target>();
			orderedNotes = new List<Target>();
			loadedNotes = new List<Target>();
			selectedNotes = new List<Target>();
		}

		public void ResetTimeline() {
			DeleteAllTargets();
			Tools.undoRedoManager.ClearActions();
			tempoChanges.Clear();
		}

		public void Export()
		{

			Debug.Log("Saving: " + audicaFile.desc.title);
			
			//Ensure all chains are generated
			List<TargetData> nonGeneratedNotes = new List<TargetData>();

			foreach(Target note in notes) {
				if(note.data.behavior == TargetBehavior.NR_Pathbuilder && note.data.pathBuilderData.createdNotes == false) {
					nonGeneratedNotes.Add(note.data);
				}
			}

			foreach(var data in nonGeneratedNotes) {
				ChainBuilder.GenerateChainNotes(data);
			}

			//Export map
			string dirpath = Application.persistentDataPath;

			CueFile export = new CueFile();
			export.cues = new List<Cue>();
			export.NRCueData = new NRCueData();

			foreach (Target target in orderedNotes) {

				if (target.data.beatLength == 0) target.data.beatLength = Constants.SixteenthNoteDuration;
				
				if (target.data.behavior == TargetBehavior.Metronome) continue;
				
				var cue = NotePosCalc.ToCue(target, offset);

				if(target.data.behavior == TargetBehavior.NR_Pathbuilder) {
					export.NRCueData.pathBuilderNoteCues.Add(cue);
					export.NRCueData.pathBuilderNoteData.Add(target.data.pathBuilderData);
					continue;
				}

				export.cues.Add(cue);
			}

			switch (difficultyManager.loadedIndex) {
				case 0:
					audicaFile.diffs.expert = export;
					break;
				case 1:
					audicaFile.diffs.advanced = export;
					break;
				case 2:
					audicaFile.diffs.moderate = export;
					break;
				case 3:
					audicaFile.diffs.beginner = export;
					break;
			}

			audicaFile.desc = desc;

			desc.tempoList = tempoChanges;
			
			AudicaExporter.ExportToAudicaFile(audicaFile);

			NotificationShower.AddNotifToQueue(new NRNotification("Map saved successfully!"));


		}

		public void ExportAndPlay() {
			Export();
			string songFolder = PathLogic.GetSongFolder();
			File.Delete(Path.Combine(songFolder, audicaFile.desc.songID + ".audica"));
			File.Copy(audicaFile.filepath, Path.Combine(songFolder, audicaFile.desc.songID + ".audica"));

			string newPath = Path.GetFullPath(Path.Combine(songFolder, @"..\..\..\..\"));
			System.Diagnostics.Process.Start(Path.Combine(newPath, "Audica.exe"));
		}


		public void LoadTimingMode(AudioClip clip) {
			if (audicaLoaded) return;

			songPlayback.LoadAudioClip(clip, PrecisePlayback.LoadType.MainSong);
			inTimingMode = true;
			audioLoaded = true;
		}

		public void CopyTimestampToClipboard() {
			string timestamp = songTimestamp.text;
			GUIUtility.systemCopyBuffer = "**" + time.tick.ToString() + "**" + " - ";
		}

		public void SetTimingModeStats(UInt64 microsecondsPerQuarterNote, int tickOffset) {
			DeleteAllTargets();

			SetBPM(new QNT_Timestamp(0), microsecondsPerQuarterNote, false);

			SafeSetTime();
		}


		public void ExitTimingMode() {

			inTimingMode = false;
			DeleteAllTargets();

		}

		public bool LoadAudicaFile(bool loadRecent = false, string filePath = null) {
			readyToRegenerate = false;

			inTimingMode = false;
			SetOffset(new Relative_QNT(0));

			if (audicaLoaded) {
				miniTimeline.ClearBookmarks(false);
			}
			
			if (audicaLoaded && NRSettings.config.saveOnLoadNew) {
				Export();
			}

			if (loadRecent) {
				audicaFile = null;
				audicaFile = AudicaHandler.LoadAudicaFile(PlayerPrefs.GetString("recentFile", null));
				if (audicaFile == null) return false;

			} else if (filePath != null) {
				audicaFile = null;
				audicaFile = AudicaHandler.LoadAudicaFile(filePath);
				PlayerPrefs.SetString("recentFile", audicaFile.filepath);

			} else {


				string prevDir = PlayerPrefs.GetString("recentDir", "");

				string[] paths;
				
				if (prevDir != "") {
					paths = StandaloneFileBrowser.OpenFilePanel("Audica File (Not OST)", prevDir, "audica", false);
					
				}
				else {
					paths = StandaloneFileBrowser.OpenFilePanel("Audica File (Not OST)", Application.dataPath, "audica", false);
				}
				
				
				

				if (paths.Length == 0) return false;
				
				
				PlayerPrefs.SetString("recentDir", Path.GetDirectoryName(paths[0]));
				
				audicaFile = null;
				
				audicaFile = AudicaHandler.LoadAudicaFile(paths[0]);
				PlayerPrefs.SetString("recentFile", paths[0]);
			}

			ResetTimeline();
			
			desc = audicaFile.desc;
			
			// Get song BPM
			if (audicaFile.song_mid != null) {
				foreach(var eventList in audicaFile.song_mid.Events) {
					foreach(var e in eventList) {
						if(e is TempoEvent) {
							TempoEvent tempo = (e as TempoEvent);
							QNT_Timestamp time = new QNT_Timestamp((UInt64)tempo.AbsoluteTime);
							SetBPM(time, (UInt64)tempo.MicrosecondsPerQuarterNote, false);
						}
					}
				}

				//Now, try to match up time signatures with existing tempo markers
				foreach(var eventList in audicaFile.song_mid.Events) {
					foreach(var e in eventList) {
						if(e is TimeSignatureEvent) {
							TimeSignatureEvent timeSignatureEvent = (e as TimeSignatureEvent);
							QNT_Timestamp time = new QNT_Timestamp((UInt64)timeSignatureEvent.AbsoluteTime);
							TimeSignature signature = new TimeSignature((uint)timeSignatureEvent.Numerator, (uint)(1 << timeSignatureEvent.Denominator));

							bool found = false;
							for(int i = 0; i < tempoChanges.Count; ++i) {
								if(tempoChanges[i].time == time) {
									TempoChange change = tempoChanges[i];
									change.timeSignature = signature;
									tempoChanges[i] = change;
									found = true;
									break;
								}
							}

							//If there is no tempo change with this time signature, add whatever the current tempo was at that point
							if(!found) {
								SetBPM(time, GetTempoForTime(time).microsecondsPerQuarterNote, false, signature);
							}
						}
					}
				}
			} 

			//If we didn't load any bpm, set it from the song desc
			int zeroBPMIndex = GetCurrentBPMIndex(new QNT_Timestamp(0));
			if(zeroBPMIndex == -1) {
				SetBPM(new QNT_Timestamp(0), Constants.MicrosecondsPerQuarterNoteFromBPM(desc.tempo), false);
			}

			//Update our discord presence
			nrDiscordPresence.UpdatePresenceSongName(desc.title);

			//Loads all the sounds.
			StartCoroutine(GetAudioClip($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}.ogg"));
			if (audicaFile.desc.sustainSongLeft != "") StartCoroutine(LoadLeftSustain($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongLeft}.ogg"));
			if (audicaFile.desc.sustainSongRight != "") StartCoroutine(LoadRightSustain($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongRight}.ogg"));
			StartCoroutine(LoadExtraAudio($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedFxSong}.ogg"));

			//foreach (Cue cue in audicaFile.diffs.expert.cues) {
			//AddTarget(cue);
			//}
			//Difficulty manager loads stuff now
			audicaLoaded = true;
			difficultyManager.LoadHighestDifficulty();

			//Disable timing window buttons so users don't mess stuff up.
			generateAudicaButton.interactable = false;
			loadAudioFileTiming.interactable = false;

			//Load bookmarks

			if (audicaFile.desc.bookmarks != null) {
				foreach (BookmarkData data in audicaFile.desc.bookmarks) {
					miniTimeline.SetBookmark(data.xPosMini, data.xPosTop, data.type, true, false);
				}
				
			}

			//Loaded successfully

			NotificationShower.AddNotifToQueue(new NRNotification("Map loaded successfully!"));
			NotificationShower.AddNotifToQueue(new NRNotification("Press F1 to view shortcuts"));
			return true;
		}


		IEnumerator GetAudioClip(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					songPlayback.LoadAudioClip(myClip, PrecisePlayback.LoadType.MainSong);
					
					audioLoaded = true;
					audicaLoaded = true;
					
					//Load the preview start point
					miniTimeline.SetPreviewStartPoint(ShiftTick(new QNT_Timestamp(0), (float)desc.previewStartSeconds));

					readyToRegenerate = true;
					RegenerateBPMTimelineData();
				}
			}
		}

		IEnumerator LoadLeftSustain(string uri) {
			Debug.Log("Loading left sustian.");
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					audicaFile.usesLeftSustain = true;
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					songPlayback.LoadAudioClip(myClip, PrecisePlayback.LoadType.LeftSustain);
				}
			}
		}
		IEnumerator LoadRightSustain(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					audicaFile.usesRightSustain = true;
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					songPlayback.LoadAudioClip(myClip, PrecisePlayback.LoadType.RightSustain);
				}
			}
		}

		IEnumerator LoadExtraAudio(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					songPlayback.LoadAudioClip(myClip, PrecisePlayback.LoadType.Extra);
				}
			}
		}
		
		public void SetPlaybackSpeed(float speed) {
			if (!audioLoaded) return;

			playbackSpeed = speed;
			songPlayback.speed = speed;
		}
		
		public void SetPlaybackSpeedFromSlider(Slider slider) {
			if (!audioLoaded) return;

			SetPlaybackSpeed(slider.value);
		}

		struct UpdateTiming {
			public TargetData data;
			public QNT_Timestamp newTime;
		}

		struct TempoFixup {
			public int tempoId;
			public float time;
		}

		//When we shift a previous tempo, we still need to keep the other tempo changes
		// at the same point in time
		List<TempoFixup> GatherTempoFixups(int tempoIndex) {
			List<TempoFixup> tempoFixes = new List<TempoFixup>();
			for(int i = tempoIndex + 1; i < tempoChanges.Count; ++i) {
				TempoFixup fixup;
				fixup.tempoId = i;
				fixup.time = TimestampToSeconds(tempoChanges[i].time);
				tempoFixes.Add(fixup);
			}

			return tempoFixes;
		}

		//Go through each future tempo, adjust it so that it is still at the same point in time, 
		// then adjust all of the notes in that tempo so that they are still aligned
		void FixupTempoTimings(List<TempoFixup> tempoFixes, List<UpdateTiming> updateTimings) {
			foreach(TempoFixup fixup in tempoFixes) {
				QNT_Timestamp newTime = ShiftTick(new QNT_Timestamp(0), fixup.time);
				TempoChange change = tempoChanges[fixup.tempoId];
				Relative_QNT changeOffset = newTime - change.time;

				QNT_Timestamp endTime = new QNT_Timestamp(UInt64.MaxValue);
				if(fixup.tempoId + 1 < tempoChanges.Count) {
					endTime = tempoChanges[fixup.tempoId + 1].time;
				}

				var enumerator = new NoteEnumerator(change.time, endTime);
				enumerator.endInclusive = false;

				foreach(Target target in enumerator) {
					UpdateTiming t;
					t.data = target.data;
					t.newTime = t.data.time + changeOffset;
					updateTimings.Add(t);
				}

				change.time = newTime;
				tempoChanges[fixup.tempoId] = change;
			}

			tempoChanges = tempoChanges.OrderBy(tempo => tempo.time.tick).ToList();
		}

		void ShiftNotesByBPM(UInt64 prevMicrosecondPerQuarterNote, QNT_Timestamp time, List<TempoFixup> tempoFixes) {
			int tempoIndex = GetCurrentBPMIndex(time);
			var newTempo = tempoChanges[tempoIndex];

			UInt64 newMicrosecondPerQuarterNote = newTempo.microsecondsPerQuarterNote;

			if(prevMicrosecondPerQuarterNote == 0) {
				if(tempoIndex > 0) {
					prevMicrosecondPerQuarterNote = tempoChanges[tempoIndex - 1].microsecondsPerQuarterNote;
				}
				//We are at the beginning, but no previous bpm. We can't shift anything
				else {
					return;
				}
			}

			float p = (float)prevMicrosecondPerQuarterNote / newMicrosecondPerQuarterNote;
			List<UpdateTiming> updateTimings = new List<UpdateTiming>();

			QNT_Timestamp recalcStart = time;
			QNT_Timestamp recalcEnd = new QNT_Timestamp(UInt64.MaxValue);
			if(tempoIndex + 1 < tempoChanges.Count) {
				recalcEnd = tempoChanges[tempoIndex + 1].time;
			}

			//Recalc all notes in the zone
			var enumerator = new NoteEnumerator(recalcStart, recalcEnd);
			enumerator.endInclusive = false;

			foreach(Target target in enumerator) {
				QNT_Duration tempoTimeDifference = new QNT_Duration(target.data.time.tick - recalcStart.tick);
				QNT_Duration duration_from_start = new QNT_Duration((UInt64)(tempoTimeDifference.tick * p));

				UpdateTiming t;
				t.data = target.data;
				t.newTime = recalcStart + duration_from_start;
				updateTimings.Add(t);
			}

			FixupTempoTimings(tempoFixes, updateTimings);

			//Update all notes
			foreach(var t in updateTimings) {
				t.data.time = t.newTime;
			}
		}

		public void ShiftNearestBPMToCurrentTime() {
			int tempoIndex = GetCurrentBPMIndex(time);
			if(tempoIndex == -1) {
				return;
			}

			Relative_QNT offset = time - tempoChanges[tempoIndex].time;
			if(tempoIndex + 1 < tempoChanges.Count) {
				Relative_QNT next = time - tempoChanges[tempoIndex + 1].time;
				if(Math.Abs(next.tick) < Math.Abs(offset.tick)) {
					tempoIndex += 1;
					offset = next;
				}
			}

			//If we try to shift the first tempo marker, don't do that
			if(tempoIndex == 0) {
				var notif = new NRNotification("Cannot shift first bpm marker!");
				notif.type = NRNotifType.Fail;
				NotificationShower.AddNotifToQueue(notif);
				return;
			}

			List<UpdateTiming> updateTimings = new List<UpdateTiming>();
			
			var nextTempo = tempoChanges[tempoIndex];
			QNT_Timestamp recalcStart = nextTempo.time;
			QNT_Timestamp recalcEnd = new QNT_Timestamp(UInt64.MaxValue);
			if(tempoIndex + 1 < tempoChanges.Count) {
				TempoChange nextChange = tempoChanges[tempoIndex + 1];
				recalcEnd = nextChange.time;
			}

			//Update the notes in the recalc area
			var enumerator = new NoteEnumerator(recalcStart, recalcEnd);
			enumerator.endInclusive = false;

			foreach(Target target in enumerator) {
				UpdateTiming t;
				t.data = target.data;
				t.newTime = t.data.time + offset;
				updateTimings.Add(t);
			}

			List<TempoFixup> tempoFixes = GatherTempoFixups(tempoIndex);

			//Change the tempo
			nextTempo.time = time;
			tempoChanges[tempoIndex] = nextTempo;

			FixupTempoTimings(tempoFixes, updateTimings);

			//Update all notes
			foreach(var t in updateTimings) {
				t.data.time = t.newTime;
			}

			RegenerateBPMTimelineData();
		}

		public void SetBPM(QNT_Timestamp time, UInt64 microsecondsPerQuarterNote, bool shiftFutureEvents, TimeSignature? signatureArg = null) {
			TimeSignature signature = signatureArg ?? new TimeSignature(4,4);

			TempoChange c = new TempoChange();
			c.time = time;
			c.microsecondsPerQuarterNote = microsecondsPerQuarterNote;
			c.timeSignature = signature;

			UInt64 prevMicrosecondPerQuarterNote = 0;

			int foundIndex = -1;
			for(int i = 0; i < tempoChanges.Count; ++i) {
				if(tempoChanges[i].time == time) {
					foundIndex = i;
					break;
				}
			}

			//Never attempt to remove the first bpm marker
			if(foundIndex == 0 && microsecondsPerQuarterNote == 0) {
				var notif = new NRNotification("Cannot remove initial bpm!");
				notif.type = NRNotifType.Fail;
				NotificationShower.AddNotifToQueue(notif);
				return;
			}

			if(foundIndex == -1 && microsecondsPerQuarterNote == 0) {
				var notif = new NRNotification("Cannot add 0 bpm!");
				notif.type = NRNotifType.Fail;
				NotificationShower.AddNotifToQueue(notif);
				return;
			}

			List<TempoFixup> tempoFixes = new List<TempoFixup>();

			//Found a bpm, set it to the new value
			if(foundIndex != -1) {
				prevMicrosecondPerQuarterNote = tempoChanges[foundIndex].microsecondsPerQuarterNote;

				//Remove marker
				if(microsecondsPerQuarterNote == 0) {
					tempoFixes = GatherTempoFixups(foundIndex);
					tempoChanges.RemoveAt(foundIndex);

					//Fixup the indices, since they're off by 1 now
					for(int i = 0; i < tempoFixes.Count; ++i) {
						TempoFixup newFixup = tempoFixes[i];
						newFixup.tempoId -= 1;
						tempoFixes[i] = newFixup;
					}
				}
				//Set to new tempo
				else {
					tempoFixes = GatherTempoFixups(foundIndex);
					tempoChanges[foundIndex] = c;
				}
			}
			else {
				int index = GetCurrentBPMIndex(time);
				tempoFixes = GatherTempoFixups(index);
				tempoChanges.Add(c);

				//Fixup the indices, since they're off by 1 now
				for(int i = 0; i < tempoFixes.Count; ++i) {
					TempoFixup newFixup = tempoFixes[i];
					newFixup.tempoId += 1;
					tempoFixes[i] = newFixup;
				}
			}

			tempoChanges = tempoChanges.OrderBy(tempo => tempo.time.tick).ToList();

			//Move all future targets back
			if(shiftFutureEvents) {
				//ShiftNotesByBPM(prevMicrosecondPerQuarterNote, time, tempoFixes);
				List<UpdateTiming> updateTimings = new List<UpdateTiming>();
				FixupTempoTimings(tempoFixes, updateTimings);

				//Update all notes
				foreach(var t in updateTimings) {
					t.data.time = t.newTime;
				}
			}

			RegenerateBPMTimelineData();
		}


		bool readyToRegenerate = false;
		public void RegenerateBPMTimelineData() {
			if(!readyToRegenerate) {
				return;
			}

			foreach(var bpm in bpmMarkerObjects) {
				Destroy(bpm);
			}
			bpmMarkerObjects.Clear();

			SetScale(scale);
			foreach(var tempo in tempoChanges) {
				var timelineBPM = Instantiate(BPM_MarkerPrefab, timelineTransformParent);
				var transform1 = timelineBPM.transform;
				transform1.localPosition = new Vector3(tempo.time.ToBeatTime(), -0.5f, 0);

				string bpm = Constants.DisplayBPMFromMicrosecondsPerQuaterNote(tempo.microsecondsPerQuarterNote);
				string timeSignature = tempo.timeSignature.ToString();

				timelineBPM.GetComponentInChildren<TextMesh>().text = bpm + "\n" + timeSignature;
				bpmMarkerObjects.Add(timelineBPM);
			}

			if(songPlayback.song == null) {
				return;
			}

			QNT_Timestamp endOfAudio = ShiftTick(new QNT_Timestamp(0), songPlayback.song.length);
			
			List<Vector3> vertices = new List<Vector3>();
			List<int> indices = new List<int>();

			TempoChange currentTempo = tempoChanges[0];

			uint barLengthIncr = 0;
			for(float t = 0; t < endOfAudio.tick;) {
				float increment = Constants.PulsesPerWholeNote / currentTempo.timeSignature.Denominator;

				int indexStart = vertices.Count;

				const float width = 0.020f;
				const float maxHeight = 0.4f;
				const float zIndex = 3;
				float start = t / (float)Constants.PulsesPerQuarterNote;
				start -= width / 2;

				float height = 0.0f;
				if(barLengthIncr == 0) {
					height = maxHeight;
				}
				else {
					height = maxHeight / 4;
				}

				//For 4/4 time, set the halfway heights
				if(currentTempo.timeSignature.Numerator == 4 && currentTempo.timeSignature.Denominator == 4) {
					if(barLengthIncr == 2) {
						height = maxHeight / 2;
					}
				}

				vertices.Add(new Vector3(start, -0.5f, zIndex));
				vertices.Add(new Vector3(start + width, -0.5f, zIndex));
				vertices.Add(new Vector3(start + width, -0.5f + height, zIndex));
				vertices.Add(new Vector3(start, -0.5f + height, zIndex));

				indices.Add(indexStart + 0);
				indices.Add(indexStart + 1);
				indices.Add(indexStart + 2);

				indices.Add(indexStart + 2);
				indices.Add(indexStart + 3);
				indices.Add(indexStart + 0);

				barLengthIncr++;
				barLengthIncr = barLengthIncr % currentTempo.timeSignature.Numerator;
				
				bool newTempo = false;
				foreach(TempoChange tempoChange in tempoChanges) {
					if(t < tempoChange.time.tick && t + increment >= tempoChange.time.tick) {
						barLengthIncr = 0;
						t = tempoChange.time.tick;
						currentTempo = tempoChange;
						newTempo = true;
						break;
					}
				}

				if(!newTempo) {
					t += increment;
				}
			}

			Mesh mesh = timelineNotesStatic.gameObject.GetComponent<MeshFilter>().mesh;
			mesh.Clear();

			mesh.vertices = vertices.ToArray();
			mesh.triangles = indices.ToArray();

			waveformVisualizer.GenerateWaveform(songPlayback.song, this);
		}

		public void SetOffset(Relative_QNT newOffset) {
			StopCoroutine(AnimateSetTime(new QNT_Timestamp(0)));
			Relative_QNT diff = offset - newOffset;
			offset = newOffset;
			
			QNT_Timestamp newTime = time + diff;
			if (newTime != time) {
				StartCoroutine(AnimateSetTime(newTime));
			}
		}

		public void SetSnap(int newSnap) {
			beatSnap = newSnap;
		}

		private bool isBeatSnapWarningActive = false;
		public void BeatSnapChanged() {
			string temp = beatSnapSelector.elements[beatSnapSelector.index];
			int snap = 4;
			int.TryParse(temp.Substring(2), out snap);
			beatSnap = snap;

			if (snap >= 32 && !isBeatSnapWarningActive) {
				beatSnapWarningText.DOFade(1f, 0.5f);
				isBeatSnapWarningActive = true;
			}
			else if (isBeatSnapWarningActive) {
				beatSnapWarningText.DOFade(0f, 0.5f);
				isBeatSnapWarningActive = false;
			}
		}

		public int GetCurrentBPMIndex(QNT_Timestamp t) {
			for(int i = 0; i < tempoChanges.Count; ++i) {
				var c = tempoChanges[i];

				if(t >= c.time && (i + 1 >= tempoChanges.Count || t < tempoChanges[i + 1].time)) {
					return i;
				}
			}

			return -1;
		}

		public TempoChange GetTempoForTime(QNT_Timestamp t) {
			int idx = GetCurrentBPMIndex(t);
			if(idx == -1) {
				TempoChange change;
				change.time = t;
				change.microsecondsPerQuarterNote = Constants.OneMinuteInMicroseconds / 60;
				change.timeSignature = new TimeSignature(4,4);
				return change;
			}

			return tempoChanges[idx];
		}

		public double GetBpmFromTime(QNT_Timestamp t) {
			int idx = GetCurrentBPMIndex(t);
			if(idx != -1) {
				return Constants.GetBPMFromMicrosecondsPerQuaterNote(tempoChanges[idx].microsecondsPerQuarterNote);
			}
			else {
				return 120.0;
			}
		}

		public void SetBeatTime(QNT_Timestamp t) {
			float x = t.ToBeatTime() - offset.ToBeatTime();

			timelineBG.material.SetTextureOffset(MainTex, new Vector2((x / 4f + scaleOffset), 1));

			timelineTransformParent.transform.localPosition = Vector3.left * x / (scale / 20f);

			gridTransformParent.transform.localPosition = Vector3.back * x;
		}

		public void SetScale(int newScale) {
			if (newScale < 5 || newScale > 100) return;
			timelineBG.material.SetTextureScale("_MainTex", new Vector2(newScale / 4f, 1));
			scaleOffset = -newScale % 8 / 8f;

			Vector3 timelineTransformScale = timelineTransformParent.transform.localScale;
			timelineTransformScale.x *= (float) scale / newScale;
			scaleTransform = timelineTransformScale.x;
			timelineTransformParent.transform.localScale = timelineTransformScale;

			targetScale *= (float) newScale / scale;
			// fix scaling on all notes
			foreach (Transform note in timelineTransformParent.transform) {
				Vector3 noteScale = note.localScale;
				noteScale.x = targetScale;
				note.localScale = noteScale;
			}


			scale = newScale;

			foreach (Target target in orderedNotes) {
				target.UpdateTimelineSustainLength();
			}
		}

		public void UpdateTrail() {
			Vector3[] positions = new Vector3[gridTransformParent.childCount];
			for (int i = 0; i < gridTransformParent.transform.childCount; i++) {
				positions[i] = gridTransformParent.GetChild(i).localPosition;
			}
			positions = positions.OrderBy(v => v.z).ToArray();
			var liner = gridTransformParent.gameObject.GetComponentInChildren<LineRenderer>();
			liner.positionCount = gridTransformParent.childCount;
			liner.SetPositions(positions);
		}

		public void EnableNearSustainButtons() {
			foreach (Target target in loadedNotes) {
				if (!target.data.supportsBeatLength) continue;

				bool shouldDisplayButton = paused; //Need to be paused
				shouldDisplayButton &= target.GetRelativeBeatTime() < 2 && target.GetRelativeBeatTime() > -2; //Target needs to be "near"

				//Be in drag select, or be a path builder note in path builder mode
				shouldDisplayButton &= EditorInput.selectedTool == EditorTool.DragSelect || (target.data.behavior == TargetBehavior.NR_Pathbuilder && EditorInput.selectedTool == EditorTool.ChainBuilder);
				
				if (shouldDisplayButton) {
					target.EnableSustainButtons();
				} else {
					target.DisableSustainButtons();
				}
			}
		}

		bool checkForNearSustainsOnThisFrame = false;
		public void Update() {
			QNT_Timestamp startTime = time;

			UpdateSustains();

			if (!paused) {
				time = ShiftTick(new QNT_Timestamp(0), (float)songPlayback.GetTime());
			}

			bool isScrollingBeatSnap = false;
			
			
			bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool isAltDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

			if (hover) {
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
					if (Input.mouseScrollDelta.y > 0.1f) {
						SetScale(scale - 1);
					} else if (Input.mouseScrollDelta.y < -0.1f) {
						SetScale(scale + 1);
					}
					SetBeatTime(time);
				}
			}

			if (isAltDown && Input.mouseScrollDelta.y < -0.1f) {
				isScrollingBeatSnap = true;
				beatSnapSelector.PreviousClick();

			} else if (isAltDown && Input.mouseScrollDelta.y > 0.11f) {
				isScrollingBeatSnap = true;
				beatSnapSelector.ForwardClick();
			}
			
			//if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && hover))

			bool dragging = Input.GetMouseButton(0) && hover;

			if (!isShiftDown && !isScrollingBeatSnap && Math.Abs(Input.mouseScrollDelta.y) > 0.1f) {
				if (!audioLoaded) return;
				if (EditorInput.inUI) return;

				Relative_QNT jumpDuration = new Relative_QNT((long)Constants.DurationFromBeatSnap((uint)beatSnap).tick);

				bool moveTick = false;
				if(isCtrlDown && !dragging) {
					moveTick = true;
					jumpDuration = new Relative_QNT(1);
				}

				if(Input.mouseScrollDelta.y >= -0.1f) {
					jumpDuration.tick *= -1;
				}

				if(!moveTick) {
					time = GetClosestBeatSnapped(time + jumpDuration, (uint)beatSnap);
				}
				else {
					time = time + jumpDuration;
				}

				SafeSetTime();
				if (paused) {
					songPlayback.PlayPreview(time, jumpDuration);
					checkForNearSustainsOnThisFrame = true;
				}
				else {
					songPlayback.Play(time);
				}

				SetBeatTime(time);

				StopCoroutine(AnimateSetTime(new QNT_Timestamp(0)));
			}

            if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) {
                CopyTimestampToClipboard();
            }

			if (!paused && !animatingTimeline) {
				SetBeatTime(time);
			}


			if (Input.GetKeyDown(KeyCode.A) && isCtrlDown) {

				Camera.main.farClipPlane = 1000;
				
				foreach (Target target in orderedNotes) {
					target.MakeTimelineSelectTarget();
				}
			}
			

			songPlayback.volume = NRSettings.config.mainVol;
			songPlayback.hitSoundVolume = NRSettings.config.noteVol;

			SetCurrentTime();
			SetCurrentTick();

			miniTimeline.SetPercentagePlayed(GetPercentagePlayed());


			EnableNearSustainButtons();

			List<Target> newLoadedNotes = new List<Target>();
			QNT_Timestamp loadStart = Timeline.time - Relative_QNT.FromBeatTime(10.0f);
			QNT_Timestamp loadEnd = Timeline.time + Relative_QNT.FromBeatTime(10.0f);
			
			foreach(Target t in new NoteEnumerator(loadStart, loadEnd)) {
				newLoadedNotes.Add(t);
			}
			loadedNotes = newLoadedNotes;

			if(startTime != time) {
				QNT_Timestamp start = startTime;
				QNT_Timestamp end = time;

				if(start > end) {
					QNT_Timestamp temp = start;
					start = end;
					end = temp;
				}

				foreach(Target t in new NoteEnumerator(start, end)) {
					t.OnNoteHit();
				}
			}


			//Update trace lines
			if(NRSettings.config.enableTraceLines) {
				UpdateTraceLine(leftHandTraceLine, TargetHandType.Left, NRSettings.config.leftColor);
				UpdateTraceLine(rightHandTraceLine, TargetHandType.Right, NRSettings.config.rightColor);
			}

			if(NRSettings.config.enableDualines) {
				foreach(var line in dualNoteTraceLines) {
					line.enabled = false;
				}

				int index = 0;
				var backIt = new NoteEnumerator(Timeline.time - Relative_QNT.FromBeatTime(0.3f), Timeline.time + Relative_QNT.FromBeatTime(1.7f));
				Target lastTarget = null;
				foreach(Target t in backIt) {
					if(lastTarget != null && 
						t.data.behavior != TargetBehavior.Chain && lastTarget.data.behavior != TargetBehavior.Chain &&
						t.data.handType != TargetHandType.Either && t.data.handType != TargetHandType.None && 
						lastTarget.data.handType != TargetHandType.Either && lastTarget.data.handType != TargetHandType.None
						) {
						TargetHandType expected = TargetHandType.Left;
						if(lastTarget.data.handType == expected) {
							expected = TargetHandType.Right;
						}

						if(t.data.time == lastTarget.data.time && t.data.handType == expected) {
							var dualNoteTraceLine = GetOrCreateDualLine(index++);
							dualNoteTraceLine.enabled = true;

							float alphaVal = 0.0f;
							if(Timeline.time > t.data.time) {
								alphaVal = 1.0f - ((Timeline.time - t.data.time).ToBeatTime() / 0.3f);
							}
							else {
								alphaVal = 1.0f - ((t.data.time - Timeline.time).ToBeatTime() / 1.7f);
							}

							Vector2 leftPos = t.data.position;
							Vector2 rightPos = lastTarget.data.position;
							if(t.data.handType == TargetHandType.Right) {
								Vector2 temp = rightPos;
								rightPos = leftPos;
								leftPos = temp;
							}

							Vector3[] positions = new Vector3[2];
							positions[0] = new Vector3(leftPos.x, leftPos.y, 0.05f);
							positions[1] = new Vector3(rightPos.x, rightPos.y, 0.05f);
							dualNoteTraceLine.positionCount = positions.Length;
							dualNoteTraceLine.SetPositions(positions);

							Gradient gradient = new Gradient();
							gradient.SetKeys(
								new GradientColorKey[] { new GradientColorKey(NRSettings.config.leftColor, 0.0f), new GradientColorKey(NRSettings.config.rightColor, 1.0f) },
								new GradientAlphaKey[] { new GradientAlphaKey(alphaVal, 0.0f), new GradientAlphaKey(alphaVal, 1.0f) }
							);
							dualNoteTraceLine.colorGradient = gradient;
						}
					}

					lastTarget = t;
				}
			}
		}

		private LineRenderer GetOrCreateDualLine(int index) {
			while(dualNoteTraceLines.Count <= index) {
				GameObject inst = GameObject.Instantiate(dualNoteTraceLinePrefab);
				var renderer = inst.GetComponent<LineRenderer>();
				renderer.enabled = false;
				dualNoteTraceLines.Add(renderer);
			}

			return dualNoteTraceLines.ElementAt(index);
		}

		private void UpdateTraceLine(LineRenderer renderer, TargetHandType handType, Color color) {
			float TraceAheadTime = 1f;

			renderer.enabled = false;
			if(paused) { return; }

			var startTime = Timeline.time + Relative_QNT.FromBeatTime(TraceAheadTime);

			Target nextTarget = null;
			foreach(Target t in new NoteEnumerator(startTime, startTime + Relative_QNT.FromBeatTime(1))) {
				if(t.data.behavior == TargetBehavior.Melee || t.data.behavior == TargetBehavior.Chain) continue;

				if(t.data.handType == handType) {
					nextTarget = t;
					break;
				}
			}

			if(nextTarget == null) {
				return;
			}

			var backIt = new NoteEnumerator(nextTarget.data.time - Relative_QNT.FromBeatTime(2), nextTarget.data.time);
			backIt.reverse = true;

			Target closest = null;
			Target startTarget = null;
			foreach(Target t in backIt) {
				if(t == nextTarget || t.data.behavior == TargetBehavior.Melee || t.data.behavior == TargetBehavior.Chain) continue;

				if(t.data.handType == handType) {
					startTarget = t;
					break;
				}

				if(closest == null) {
					closest = t;
				}
			}

			if(startTarget == null) {
				if(closest == null) {
					return;
				}

				startTarget = closest;
			}

			if(nextTarget != null) {
				float NoteFadeInTime = 1.0f;
				float dist = (nextTarget.data.time - startTarget.data.time).ToBeatTime();
				float travelTime = (nextTarget.data.time - startTime).ToBeatTime();
				if(dist <= 2f && travelTime < dist && travelTime > 0.0 && travelTime < NoteFadeInTime) {
					float totalTime = Math.Min(NoteFadeInTime, dist);
					float percent = 1.0f - ((travelTime) / totalTime);

					renderer.enabled = true;

					Vector2 start = startTarget.data.position + (nextTarget.data.position - startTarget.data.position) * Easings.Linear(percent);
					Vector2 end = startTarget.data.position + (nextTarget.data.position - startTarget.data.position);

					Vector3[] positions = new Vector3[2];
					positions[0] = new Vector3(start.x, start.y, 0.0f);
					positions[1] = new Vector3(end.x, end.y, 0.0f);
					renderer.positionCount = positions.Length;
					renderer.SetPositions(positions);

					Gradient gradient = new Gradient();
					gradient.SetKeys(
						new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 0.25f), new GradientColorKey(color, 1.0f) },
						new GradientAlphaKey[] { new GradientAlphaKey(0.1f, 0.0f), new GradientAlphaKey(0.25f, 0.25f), new GradientAlphaKey(0.5f, 1.0f) }
					);
					renderer.colorGradient = gradient;
				}
			}
		}

		public double GetPercentPlayedFromSeconds(double seconds)
		{
			return seconds / songPlayback.song.length;
		}


		public void JumpToPercent(float percent) {
			if (!audioLoaded) return;

			time = ShiftTick(new QNT_Timestamp(0), songPlayback.song.length * percent);

			SafeSetTime();
			SetCurrentTime();
			SetCurrentTick();

			SetBeatTime(time);
			songPlayback.PlayPreview(time, new Relative_QNT((long)Constants.DurationFromBeatSnap((uint)beatSnap).tick));
		}

		public void JumpToX(float x) {
			StopCoroutine(AnimateSetTime(new QNT_Timestamp(0)));

			float posX = Math.Abs(timelineTransformParent.position.x) + x;
			QNT_Timestamp newTime = new QNT_Timestamp(0) + QNT_Duration.FromBeatTime(posX * (scale / 20f));
			newTime = GetClosestBeatSnapped(newTime, (uint)beatSnap);
			SafeSetTime();

			StartCoroutine(AnimateSetTime(newTime));
		}

		public void ToggleWaveform() {
			waveformVisualizer.visible = !waveformVisualizer.visible;
		}


		public void TogglePlayback() {
			if (!audioLoaded) return;

			bool isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

			if (paused) {
				//gameObject.GetComponent<AudioSource>().Play();
				//aud.Play();
				//previewAud.Pause();

				if(isCtrlDown) {
					songPlayback.StartMetronome();
				}

				songPlayback.Play(time);
				paused = false;
			} else {
				//aud.Pause();
				songPlayback.Stop();
				paused = true;

				//Snap to the beat snap when we pause
				time = GetClosestBeatSnapped(time, (uint)beatSnap);

				float currentTimeSeconds = TimestampToSeconds(time);
				if (currentTimeSeconds > songPlayback.song.length) {
					time = ShiftTick(new QNT_Timestamp(0), songPlayback.song.length);
				}

				SetBeatTime(time);
				SafeSetTime();
				SetCurrentTick();
				SetCurrentTime();
			}
		}

		public void SafeSetTime() {
			if (time.tick < 0) time = new QNT_Timestamp(0);
			if (!audioLoaded) return;

			float currentTimeSeconds = TimestampToSeconds(time);

			if (currentTimeSeconds > songPlayback.song.length) {
				time = ShiftTick(new QNT_Timestamp(0), songPlayback.song.length);
				currentTimeSeconds = songPlayback.song.length;
			}
		}

		public IEnumerator AnimateSetTime(QNT_Timestamp newTime) {

			animatingTimeline = true;

			if (!audioLoaded) yield break;

			if (TimestampToSeconds(newTime) > songPlayback.song.length) {
				newTime = ShiftTick(new QNT_Timestamp(0), songPlayback.song.length);
			}

			//DOTween.Play
			DOTween.To(t => SetBeatTime(new QNT_Timestamp((UInt64)Math.Round(t))), time.tick, newTime.tick, 0.2f).SetEase(Ease.InOutCubic);

			yield return new WaitForSeconds(0.2f);

			time = newTime;
			animatingTimeline = false;

			SafeSetTime();
			SetBeatTime(time);

			SetCurrentTime();
			SetCurrentTick();

			songPlayback.PlayPreview(time, new Relative_QNT((long)Constants.DurationFromBeatSnap((uint)beatSnap).tick));

			yield break;

		}

		void OnMouseEnter() {
			hover = true;
		}

		void OnMouseExit() {
			hover = false;
		}

		//Snap (rounded down) to the nearest beat given by `beatSnap`
		public QNT_Timestamp GetClosestBeatSnapped(QNT_Timestamp timeToSnap, uint beatSnap) {
			int tempoIndex = GetCurrentBPMIndex(timeToSnap);
			if(tempoIndex == -1) {
				return QNT_Timestamp.GetSnappedValue(timeToSnap, beatSnap);
			}

			TempoChange currentTempo = tempoChanges[tempoIndex];
			QNT_Duration offsetFromTempoChange = new QNT_Duration(timeToSnap.tick - currentTempo.time.tick);
			offsetFromTempoChange = QNT_Duration.GetSnappedValue(offsetFromTempoChange, beatSnap);
			return currentTempo.time + offsetFromTempoChange;
		}

		private void OnMouseDown() {
			//We don't want to interfere with drag select
			if (EditorInput.selectedTool == EditorTool.DragSelect) return;
			JumpToX(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
		}

		public float GetPercentagePlayed() {
			if (songPlayback.song != null)
				return (TimestampToSeconds(time) / songPlayback.song.length);

			else
				return 0;
		}

		//Shifts `startTime` by `duration` seconds, respecting bpm changes in between
		public QNT_Timestamp ShiftTick(QNT_Timestamp startTime, float duration) {
			int currentBpmIdx = GetCurrentBPMIndex(startTime);
			if(currentBpmIdx == -1) {
				return startTime;
			}

			QNT_Timestamp currentTime = startTime;

			while(duration != 0 && currentBpmIdx >= 0 && currentBpmIdx < tempoChanges.Count) {
				var tempo = tempoChanges[currentBpmIdx];

				Relative_QNT remainingTime = Conversion.ToQNT(duration, tempo.microsecondsPerQuarterNote);
				QNT_Timestamp timeOfNextBPM = new QNT_Timestamp(0);
				int sign = Math.Sign(remainingTime.tick);

				currentBpmIdx += sign;
				if(currentBpmIdx > 0 && currentBpmIdx < tempoChanges.Count) {
					timeOfNextBPM = tempoChanges[currentBpmIdx].time;
				}

				//If there is time to another bpm we need to shift to the next bpm point, then continue
				if(timeOfNextBPM.tick != 0 && timeOfNextBPM < (currentTime + remainingTime)) {
					Relative_QNT timeUntilTempoShift = timeOfNextBPM - currentTime;
					currentTime += timeUntilTempoShift;
					duration -= (float)Conversion.FromQNT(timeUntilTempoShift, tempo.microsecondsPerQuarterNote);
				}
				//No bpm change, apply the time and break
				else {
					currentTime += remainingTime;
					break;
				}
			}

			return currentTime;
		}

		public float TimestampToSeconds(QNT_Timestamp timestamp) {
			double duration = 0.0f;

			for(int i = 0; i < tempoChanges.Count; ++i) {
				var c = tempoChanges[i];
				
				if(timestamp >= c.time && (i + 1 >= tempoChanges.Count || timestamp < tempoChanges[i + 1].time)) {
					duration += Conversion.FromQNT(timestamp - c.time, c.microsecondsPerQuarterNote);
					break;
				}
				else if(i + 1 < tempoChanges.Count) {
					duration += Conversion.FromQNT(tempoChanges[i + 1].time - c.time, c.microsecondsPerQuarterNote);
				}
			}

			return (float)duration;
		}

		string prevTimeText;
		private void SetCurrentTime() {
			float timeSeconds = TimestampToSeconds(time);

			string minutes = Mathf.Floor((int) timeSeconds / 60).ToString("00");
			string seconds = ((int) timeSeconds % 60).ToString("00");
			if (seconds != prevTimeText) {
				prevTimeText = seconds;
				songTimestamp.text = "<mspace=.5em>" + minutes + "</mspace>" + "<mspace=.4em>:</mspace>" + "<mspace=.5em>" + seconds + "</mspace>";
			}

		}

		private string prevTickText;

		private void SetCurrentTick() {
			string currentTick = time.tick.ToString();
			if (currentTick != prevTickText) {
				prevTickText = currentTick;
				curTick.text = "<mspace=.5em>" + currentTick + "</mspace>";
			}
		}

		private void SetAudioDSP() {
			//Pull DSP setting from config
			var configuration = AudioSettings.GetConfiguration();
			configuration.dspBufferSize = NRSettings.config.audioDSP;
			AudioSettings.Reset(configuration);
		}



		public void PreviewCountIn(uint beats) {
			if(!paused) {
				TogglePlayback();
			}

			time = new QNT_Timestamp(0);
			SafeSetTime();

			TempoChange first = tempoChanges[0];
			QNT_Duration timeSignatureDuration = new QNT_Duration(Constants.PulsesPerWholeNote / first.timeSignature.Denominator) * beats;
			songPlayback.PlayClickTrack(new QNT_Timestamp(0) + timeSignatureDuration);
			if(paused) {
				TogglePlayback();
			}
		}

		bool convertWavToOgg(string wavPath, string oggPath) {
			System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process();
			string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpeg.exe");
			ffmpeg.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.FileName = ffmpegPath;
			ffmpeg.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.UseShellExecute = false;
			ffmpeg.StartInfo.RedirectStandardOutput = true;
			ffmpeg.StartInfo.RedirectStandardError = true;

			ffmpeg.StartInfo.Arguments = String.Format("-y -i \"{0}\" \"{1}\"", wavPath, oggPath);
			ffmpeg.Start();
			Debug.Log(ffmpeg.StandardOutput.ReadToEnd());
			Debug.Log(ffmpeg.StandardError.ReadToEnd());
			ffmpeg.WaitForExit();

			return ffmpeg.ExitCode == 0;
		}

		void ConvertOggToMogg(string oggPath, string moggPath) {
			var workFolder = Path.Combine(Application.streamingAssetsPath, "Ogg2Audica");

			System.Diagnostics.Process ogg2mogg = new System.Diagnostics.Process();
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			startInfo.FileName = Path.Combine(workFolder, "ogg2mogg.exe");
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			
			string args = $"\"{oggPath}\" \"{moggPath}\"";
			startInfo.Arguments = args;
			startInfo.UseShellExecute = false;

			ogg2mogg.StartInfo = startInfo;
			ogg2mogg.Start();
			Debug.Log(ogg2mogg.StandardOutput.ReadToEnd());
			Debug.Log(ogg2mogg.StandardError.ReadToEnd());
			ogg2mogg.WaitForExit();
		}

		public void GenerateCountIn(uint beats) {
			TempoChange first = tempoChanges[0];
			QNT_Duration timeSignatureDuration = new QNT_Duration(Constants.PulsesPerWholeNote / first.timeSignature.Denominator) * beats;
			string appPath = Application.dataPath;
			string wavPath = $"{appPath}/.cache/" + "clickTrack.wav";
			string oggPath = $"{appPath}/.cache/" + "clickTrack.ogg";

			string moggName = "song_extras.mogg";
			string moggPath = $"{appPath}/.cache/" + moggName;
			SavWav.Save(wavPath, songPlayback.GenerateClickTrack(new QNT_Timestamp(0) + timeSignatureDuration));

			//Convert wav to ogg
			if(!convertWavToOgg(wavPath, oggPath)) {
				return;
			}

			//Convert ogg to mogg
			ConvertOggToMogg(oggPath, moggPath);

			//Add extra to zip archive
			using(var archive = ZipArchive.Open(audicaFile.filepath)) {
				foreach(ZipArchiveEntry entry in archive.Entries) {
					if (entry.ToString() == moggName) {
						archive.RemoveEntry(entry);
					}
				}
				archive.AddEntry(moggName, moggPath);
				archive.SaveTo(audicaFile.filepath + ".temp", SharpCompress.Common.CompressionType.None);
				archive.Dispose();
			}
			File.Delete(audicaFile.filepath);
			File.Move(audicaFile.filepath + ".temp", audicaFile.filepath);

			//Load the generated extra sounds
			StartCoroutine(LoadExtraAudio($"file://{oggPath}"));
		}

		public void ShiftEverythingByTime(Relative_QNT shift_amount) {
			//Shift tempo markers
			for(int i = 0; i < tempoChanges.Count; ++i) {
				TempoChange newChange = tempoChanges[i];
				if(newChange.time.tick != 0) {
					newChange.time += shift_amount;
				}

				tempoChanges[i] = newChange;
			}

			//Shift notes
			foreach(Target note in orderedNotes) {
				note.data.time += shift_amount;
			}
		}

		public void RemoveOrAddTimeToAudio(Relative_QNT timeChange) {
			string appPath = Application.dataPath;
			string mainSongPath = $"{appPath}/.cache/" + $"{audicaFile.desc.cachedMainSong}";
			string leftSustatinPath = $"{appPath}/.cache/" + $"{audicaFile.desc.cachedSustainSongLeft}";
			string rightSustatinPath = $"{appPath}/.cache/" + $"{audicaFile.desc.cachedSustainSongRight}";
			string extraSongPath = $"{appPath}/.cache/" + $"{audicaFile.desc.cachedFxSong}";

			double beatTimeChange = Conversion.FromQNT(timeChange, tempoChanges[0].microsecondsPerQuarterNote);
			Func<ClipData, string, bool> modifyAudio = (ClipData data, string basePath) => {
				if(data == null || data.samples.Length == 0) {
					return false;
				}

				SavWav.WavModificationOptions options = new SavWav.WavModificationOptions();
				int samples = (int)Math.Round(beatTimeChange * data.frequency * data.channels);
				if(samples > 0) {
					options.silenceSamples = (uint)samples;
				}
				else {
					options.trimSamples = (uint)-samples;
				}

				SavWav.AudioClipData audioData = new SavWav.AudioClipData();
				audioData.samples = data.samples;
				audioData.frequency = (uint)data.frequency;
				audioData.channels = (ushort)data.channels;

				SavWav.Save(basePath + ".wav", audioData, options);

				if(convertWavToOgg(basePath + ".wav", basePath + ".ogg")) {
					File.Delete(basePath + ".wav");
					return true;
				}

				return false;
			};

			bool modificationSucceeded = modifyAudio(songPlayback.song, mainSongPath);
			bool leftSustainSucceeded = modifyAudio(songPlayback.leftSustain, leftSustatinPath);
			bool rightSustainSucceeded = modifyAudio(songPlayback.rightSustain, rightSustatinPath);
			bool extraSongSucceeded = modifyAudio(songPlayback.songExtra, extraSongPath);
			//If success, Shift, then reload audio
			if(modificationSucceeded) {
				//Convert ogg to mogg
				ConvertOggToMogg(mainSongPath + ".ogg", $"{appPath}/.cache/" + $"{audicaFile.desc.moggMainSong}");

				HashSet<string> entriesToUpdate = new HashSet<string>();
				entriesToUpdate.Add(audicaFile.desc.moggMainSong);

				if(leftSustainSucceeded) {
					ConvertOggToMogg(leftSustatinPath + ".ogg", $"{appPath}/.cache/" + $"{audicaFile.desc.moggSustainSongLeft}");
					entriesToUpdate.Add(audicaFile.desc.moggSustainSongLeft);
				}

				if(rightSustainSucceeded) {
					ConvertOggToMogg(rightSustatinPath + ".ogg", $"{appPath}/.cache/" + $"{audicaFile.desc.moggSustainSongRight}");
					entriesToUpdate.Add(audicaFile.desc.moggSustainSongRight);
				}

				if(extraSongSucceeded) {
					ConvertOggToMogg(extraSongPath + ".ogg", $"{appPath}/.cache/" + $"{audicaFile.desc.moggFxSong}");
					entriesToUpdate.Add(audicaFile.desc.moggFxSong);
				}

				//Add extra to zip archive
				using(var archive = ZipArchive.Open(audicaFile.filepath)) {
					foreach(ZipArchiveEntry entry in archive.Entries) {
						if (entriesToUpdate.Contains(entry.ToString())) {
							archive.RemoveEntry(entry);
						}
					}

					archive.AddEntry(audicaFile.desc.moggMainSong, $"{appPath}/.cache/" + $"{audicaFile.desc.moggMainSong}");

					if(leftSustainSucceeded) {
						archive.AddEntry(audicaFile.desc.moggSustainSongLeft, $"{appPath}/.cache/" + $"{audicaFile.desc.moggSustainSongLeft}");
					}

					if(rightSustainSucceeded) {
						archive.AddEntry(audicaFile.desc.moggSustainSongRight, $"{appPath}/.cache/" + $"{audicaFile.desc.moggSustainSongRight}");
					}

					if(extraSongSucceeded) {
						archive.AddEntry(audicaFile.desc.moggFxSong, $"{appPath}/.cache/" + $"{audicaFile.desc.moggFxSong}");
					}

					archive.SaveTo(audicaFile.filepath + ".temp", SharpCompress.Common.CompressionType.None);
					archive.Dispose();
				}
				File.Delete(audicaFile.filepath);
				File.Move(audicaFile.filepath + ".temp", audicaFile.filepath);

				//After we have the new audica file, move the notes and load the new audio
				ShiftEverythingByTime(timeChange);

				audioLoaded = false;
				audicaLoaded = false;
				StartCoroutine(GetAudioClip($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}.ogg"));

				if(leftSustainSucceeded) {
					StartCoroutine(LoadLeftSustain($"file://{leftSustatinPath}.ogg"));
				}

				if(rightSustainSucceeded) {
					StartCoroutine(LoadRightSustain($"file://{rightSustatinPath}.ogg"));
				}

				if(extraSongSucceeded) {
					StartCoroutine(LoadExtraAudio($"file://{extraSongPath}.ogg"));
				}
			}
		}
	}
}