using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Michsky.UI.ModernUIPack;
using NotReaper.Grid;
using NotReaper.IO;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.Tools;
using NotReaper.UI;
using NotReaper.UserInput;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NotReaper {


	public class Timeline : MonoBehaviour {

		//Hidden public values
		[HideInInspector] public static AudicaFile audicaFile;

		[HideInInspector] public static SongDesc desc;

		[Header("Audio Stuff")]

		[SerializeField] private AudioSource aud;
		[SerializeField] private AudioSource previewAud;
		[SerializeField] private AudioSource leftSustainAud;
		[SerializeField] private AudioSource rightSustainAud;
		[SerializeField] private Transform spectrogram;

		[Header("UI Elements")]
		[SerializeField] private MiniTimeline miniTimeline;
		[SerializeField] private TextMeshProUGUI songTimestamp;
		[SerializeField] private TextMeshProUGUI curTick;
		[SerializeField] private TextMeshProUGUI curSongName;
		[SerializeField] private TextMeshProUGUI curSongDiff;

		[SerializeField] private HorizontalSelector beatSnapSelector;

		[Header("Prefabs")]
		public TargetIcon timelineTargetIconPrefab;
		public TargetIcon gridTargetIconPrefab;

		[Header("Extras")]
		[SerializeField] public EditorToolkit Tools;
		[SerializeField] private Transform timelineTransformParent;
		[SerializeField] private Transform gridTransformParent;
		public static Transform gridNotesStatic;
		public static Transform timelineNotesStatic;
		[SerializeField] private Renderer timelineBG;

		[Header("Configuration")]
		public float playbackSpeed = 1f;
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

		public static float time { get; set; }

		private int beatSnap = 4;
		[HideInInspector] public static int scale = 20;
		private float targetScale = 0.7f;
		private float scaleOffset = 0;
		private static float bpm = 60;
		private static int offset = 0;

		[HideInInspector] public bool hover = false;
		public bool paused = true;

		//Tools
		private void Start() {

			//Load the config file
			NRSettings.LoadSettingsJson();


			notes = new List<Target>();
			orderedNotes = new List<Target>();
			loadedNotes = new List<Target>();
			selectedNotes = new List<Target>();

			//securityRules.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow));

			gridNotesStatic = gridTransformParent;
			timelineNotesStatic = timelineTransformParent;

			//Modify the note colors


			
			


			StartCoroutine(CalculateNoteCollidersEnabled());

		}


		public void UpdateUIColors() {
			curSongDiff.color = NRSettings.config.rightColor;
			leftColor = NRSettings.config.leftColor;
			rightColor = NRSettings.config.rightColor;
			bothColor = UserPrefsManager.bothColor;
			neitherColor = UserPrefsManager.neitherColor;
		}

		void OnApplicationQuit() {
			//DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "\\temp\\");
			//dir.Delete(true);
		}

		//When loading from cues, use this.
		public void AddTarget(Cue cue) {
			Vector2 pos = NotePosCalc.PitchToPos(cue);

			if (cue.tickLength / 480 >= 1)
				//AddTarget(pos.x, pos.y, (cue.tick - offset) / 480f, cue.tickLength, cue.velocity, cue.handType, cue.behavior, false);
				AddTarget(pos.x, pos.y, (cue.tick - offset) / 480f, false, false, cue.tickLength, cue.velocity, cue.handType, cue.behavior);
			else
				AddTarget(pos.x, pos.y, (cue.tick - offset) / 480f, false, false, cue.tickLength, cue.velocity, cue.handType, cue.behavior);
		}


		//Use when adding a singular target to the project (from the user)
		public void AddTarget(float x, float y) {
			AddTarget(x, y, BeatTime(), true, true);
		}

		//Use for adding a target from redo/undo
		public void AddTarget(Target target, bool genUndoAction = true) {
			AddTarget(target.gridTargetPos.x, target.gridTargetPos.y, target.gridTargetPos.z, false, genUndoAction, target.beatLength, target.velocity, target.handType, target.behavior);
		}

		//When adding many notes at once from things such as copy paste.
		public void AddTargets(List<Target> targets, bool genUndoAction = true, bool autoSelectNewNotes = false) {

			List<Target> undoTargets = new List<Target>();

			//We need to generate a custom undo action (if requested), so we set the default undo generation to false.
			foreach (Target target in targets) {
				AddTarget(target.gridTargetPos.x, target.gridTargetPos.y, target.gridTargetPos.z, false, false, target.beatLength, target.velocity, target.handType, target.behavior);

				if (autoSelectNewNotes) SelectTarget(notes.Last());
				if (genUndoAction) undoTargets.Add(notes.Last());

			}

			if (genUndoAction) {
				var action = new NRActionMultiAddNote();
				action.affectedTargets = undoTargets;
				Tools.undoRedoManager.AddAction(action, true);
			}
		}


		/// <summary>
		/// Adds a target to the timeline.
		/// </summary>
		/// <param name="x">x pos</param>
		/// <param name="y">y pos</param>
		/// <param name="beatTime">Beat time to add the note to</param>
		/// <param name="userAdded">If true, fetch values for currently selected note type from EditorInput, if not, use values provided in function.</param>
		/// <param name="beatLength"></param>
		/// <param name="velocity"></param>
		/// <param name="handType"></param>
		/// <param name="behavior"></param>
		public void AddTarget(float x, float y, float beatTime, bool userAdded = true, bool genUndoAction = true, float beatLength = 0.25f, TargetVelocity velocity = TargetVelocity.Standard, TargetHandType handType = TargetHandType.Left, TargetBehavior behavior = TargetBehavior.Standard) {

			//TargetHandType type = 

			Target target = new Target();
			target.handType = userAdded ? EditorInput.selectedHand : handType;

			target.timelineTargetIcon = Instantiate(timelineTargetIconPrefab, timelineTransformParent);
			target.timelineTargetIcon.transform.localPosition = new Vector3(beatTime, 0, 0);
			target.timelineTargetIcon.transform.localScale = targetScale * Vector3.one;
			UpdateTimelineOffset(target);

			target.gridTargetIcon = Instantiate(gridTargetIconPrefab, gridTransformParent);
			target.gridTargetIcon.transform.localPosition = new Vector3(x, y, beatTime);


			//Use when the rest of the inputs aren't supplied, get them from the EditorInput script.
			if (userAdded) {

				target.SetHandType(EditorInput.selectedHand);

				target.SetBehavior(EditorInput.selectedBehavior);

				switch (EditorInput.selectedVelocity) {
					case UITargetVelocity.Standard:
						target.SetVelocity(TargetVelocity.Standard);
						break;

					case UITargetVelocity.Snare:
						target.SetVelocity(TargetVelocity.Snare);
						break;

					case UITargetVelocity.Percussion:
						target.SetVelocity(TargetVelocity.Percussion);
						break;

					case UITargetVelocity.ChainStart:
						target.SetVelocity(TargetVelocity.ChainStart);
						break;

					case UITargetVelocity.Chain:
						target.SetVelocity(TargetVelocity.Chain);
						break;

					case UITargetVelocity.Melee:
						target.SetVelocity(TargetVelocity.Melee);
						break;

					default:
						target.SetVelocity(TargetVelocity.Standard);
						break;

				}
			}

			//If userAdded false, we use the supplied inputs to generate the note.
			else {
				target.SetHandType(handType);

				target.SetBehavior(behavior);

				target.SetVelocity(velocity);
			}


			if (target.behavior == TargetBehavior.Hold) {
				target.SetBeatLength(beatLength);
			} else {
				target.SetBeatLength(0.25f);
			}


			//Now that all initial dependencies are met, we can init the target. (Loads sustain controller and outline color)
			target.Init();

			notes.Add(target);

			orderedNotes = notes.OrderBy(v => v.gridTargetIcon.transform.position.z).ToList();


			//Subscribe to the delete note event so we can delete it if the user wants. And other events.
			target.DeleteNoteEvent += DeleteTarget;

			target.TargetEnterLoadedNotesEvent += AddLoadedNote;
			target.TargetExitLoadedNotesEvent += RemoveLoadedNote;

			target.TargetSelectEvent += SelectTarget;
			target.TargetDeselectEvent += DeselectTarget;

			target.MakeTimelineUpdateSustainLengthEvent += UpdateSustainLength;


			if (genUndoAction) {
				//Add to undo redo manager if requested.
				var action = new NRActionAddNote();
				action.affectedTarget = notes.Last();

				//If the user added the note, we clear the redo actions. If the manager added it, we don't clear redo actions.
				Tools.undoRedoManager.AddAction(action, userAdded);

			}

			//EnableNearSustainButtons();
		}


		private void UpdateSustains() {


			foreach (var note in loadedNotes) {
				if (note.behavior == TargetBehavior.Hold) {
					if ((note.gridTargetIcon.transform.position.z < 0) && (note.gridTargetIcon.transform.position.z + note.beatLength / 480f > 0)) {

						var particles = note.gridTargetIcon.GetComponentInChildren<ParticleSystem>();
						if (!particles.isEmitting) {
							particles.Play();

							float panPos = (float)(note.gridTargetIcon.transform.position.x / 7.15);
							if (note.handType == TargetHandType.Left) {
								leftSustainAud.volume = sustainVolume;
								leftSustainAud.panStereo = panPos;

							} else if (note.handType == TargetHandType.Right) {
								rightSustainAud.volume = sustainVolume;
								rightSustainAud.panStereo = panPos;
							}


							var main = particles.main;
							main.startColor = note.handType == TargetHandType.Left ? new Color(leftColor.r, leftColor.g, leftColor.b, 1) : new Color(rightColor.r, rightColor.g, rightColor.b, 1);
						}

						ParticleSystem.Particle[] parts = new ParticleSystem.Particle[particles.particleCount];
						particles.GetParticles(parts);

						for (int i = 0; i < particles.particleCount; ++i) {
							parts[i].position = new Vector3(parts[i].position.x, parts[i].position.y, 0);
						}

						particles.SetParticles(parts, particles.particleCount);

					} else {
						var particles = note.gridTargetIcon.GetComponentInChildren<ParticleSystem>();
						if (particles.isEmitting) {
							particles.Stop();
							if (note.handType == TargetHandType.Left) {
								leftSustainAud.volume = 0.0f;
							} else if (note.handType == TargetHandType.Right) {
								rightSustainAud.volume = 0.0f;
							}
						}
					}
				}
			}
		}

		//Calculate the note offset for visual purpose on the timeline.
		private void UpdateTimelineOffset(Target target) {
			float xOffset = target.timelineTargetIcon.transform.localPosition.x;
			float yOffset = 0;
			float zOffset = 0;

			switch (target.handType) {
				case TargetHandType.Left:
					yOffset = 0.1f;
					zOffset = 0.1f;
					break;
				case TargetHandType.Right:
					yOffset = -0.1f;
					zOffset = 0.2f;
					break;
			}

			target.timelineTargetIcon.transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
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
			if (target.behavior != TargetBehavior.Hold) return;

			if (increase) {
				target.UpdateSustainBeatLength(target.beatLength += (480 / beatSnap) * 4f);
			} else {
				target.UpdateSustainBeatLength(Mathf.Max(target.beatLength -= (480 / beatSnap) * 4f, 0));
			}
		}

		// Invert the selected targets' colour
		public void SwapTargets(List<Target> targets, bool genUndoAction = true, bool clearRedoActions = true) {
			if (genUndoAction) {
				var action = new NRActionSwapNoteColors();
				action.affectedTargets = targets;

				Tools.undoRedoManager.AddAction(action, clearRedoActions);
			}

			targets.ForEach((Target target) => {
				switch(target.handType) {
					case TargetHandType.Left:  target.SetHandType(TargetHandType.Right); break;
					case TargetHandType.Right: target.SetHandType(TargetHandType.Left);  break;
				}
				UpdateTimelineOffset(target);
			});
		}

		// Flip the selected targets on the grid about the X
		public void FlipTargetsHorizontal(List<Target> targets, bool genUndoAction = true, bool clearRedoActions = true) {
			if (genUndoAction) {
				var action = new NRActionHFlipNotes();
				action.affectedTargets = targets;

				Tools.undoRedoManager.AddAction(action, clearRedoActions);
			}

			targets.ForEach(target => {
				var pos = target.gridTargetIcon.transform.localPosition;
				target.gridTargetIcon.transform.localPosition = new Vector3(
					pos.x * -1,
					pos.y,
					pos.z
				);
			});
		}

		// Flip the selected targets on the grid about the Y
		public void FlipTargetsVertical(List<Target> targets, bool genUndoAction = true, bool clearRedoActions = true) {
			if (genUndoAction) {
				var action = new NRActionVFlipNotes();
				action.affectedTargets = targets;

				Tools.undoRedoManager.AddAction(action, clearRedoActions);
			}

			targets.ForEach(target => {
				var pos = target.gridTargetIcon.transform.localPosition;
				target.gridTargetIcon.transform.localPosition = new Vector3(
					pos.x,
					pos.y * -1,
					pos.z
				);
			});
		}


		public void DeleteTarget(Target target, bool genUndoAction = true, bool clearRedoActions = true) {

			if (target == null) return;

			if (genUndoAction) {
				var action = new NRActionRemoveNote();
				action.affectedTarget = target;

				Tools.undoRedoManager.AddAction(action, clearRedoActions);
			}

			notes.Remove(target);
			orderedNotes.Remove(target);
			loadedNotes.Remove(target);

			if (target.gridTargetIcon)
				Destroy(target.gridTargetIcon.gameObject);

			if (target.timelineTargetIcon)
				Destroy(target.timelineTargetIcon.gameObject);

			target = null;


			//UpdateChainConnectors();
			//UpdateChords();

		}

		public void DeleteTargets(List<Target> targets, bool genUndoAction = true, bool clearRedoActions = true) {

			//We need to add a custom undo action, so we skip the default one.
			foreach (Target target in targets) {
				DeleteTarget(target, false);
			}

			if (genUndoAction) {
				var action = new NRActionMultiRemoveNote();
				action.affectedTargets = targets;
				Tools.undoRedoManager.AddAction(action, clearRedoActions);
			}


		}

		private void DeleteAllTargets() {
			foreach (Target target in notes) {
				//if (obj)
				Destroy(target.gridTargetIcon.gameObject);
				Destroy(target.timelineTargetIcon.gameObject);
			}

			//Second check through ordered notes to make sure they're all gone.
			foreach (Target obj in orderedNotes) {
				if (obj.gridTargetIcon)
					Destroy(obj.gridTargetIcon.gameObject);
				if (obj.timelineTargetIcon)
					Destroy(obj.timelineTargetIcon.gameObject);
			}

			//foreach (TimelineTarget obj in notesTimeline) {
			//    if (obj)
			//       Destroy(obj.gameObject);
			//}

			notes = new List<Target>();
			orderedNotes = new List<Target>();
			loadedNotes = new List<Target>();
			selectedNotes = new List<Target>();

			//var liner = gridTransformParent.gameObject.GetComponentInChildren<LineRenderer>();
			//if (liner) {
			//	liner.SetPositions(new Vector3[0]);
			//	liner.positionCount = 0;
			//}

			//time = 0;
		}

		public void ChangeDifficulty() {
			// Debug.Log("ChangeDifficulty");
			// string dirpath = Application.persistentDataPath;

			// //string[] cuePath = Directory.GetFiles(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues");
			// if (cuePath.Length > 0) {
			// 	DeleteAllTargets();

			// 	//load cues from temp
			// 	string json = File.ReadAllText(cuePath[0]);
			// 	json = json.Replace("cues", "Items");
			// 	Cue[] cues = new Cue[2]; //JsonUtility.FromJson<Cue>(json);
			// 	foreach (Cue cue in cues) {
			// 		AddTarget(cue);
			// 	}
			// } else {
			// 	DeleteAllTargets();
			// }

			// foreach (GridTarget obj in notes) {
			// 	Debug.Log(obj);
			// }
		}

		public void Import() {
			DeleteAllTargets();

			//load cues from temp
			var cueFiles = StandaloneFileBrowser.OpenFilePanel("Import .cues", Application.persistentDataPath, "cues", false);
			if (cueFiles.Length > 0) {
				string json = File.ReadAllText(cueFiles[0]);
				json = json.Replace("cues", "Items");
				Cue[] cues = new Cue[2]; //JsonUtility.FromJson<Cue>(json);
				foreach (Cue cue in cues) {
					AddTarget(cue);
				}

				//isSongLoaded = true;
			} else {
				Debug.Log("cues not found");

			}

		}

		public void Save() {
			Cue[] cues = new Cue[notes.Count];
			for (int i = 0; i < notes.Count; i++) {
				//cues[i] = orderedNotes[i].ToCue(offset);
			}

			string json = JsonUtility.ToJson(cues, true);
			json = json.Replace("Items", "cues");

			string dirpath = Application.persistentDataPath;

			if (notes.Count > 0)
				//File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);

				//json = JsonUtility.ToJson(songDesc, true);
			File.WriteAllText(Path.Combine(dirpath + "\\temp\\", "song.desc"), json);
			FileInfo descFile = new FileInfo(Path.Combine(dirpath + "\\temp\\", "song.desc"));


			string[] oggPath = Directory.GetFiles(dirpath + "\\temp\\", "*.ogg");
			FileInfo oggFile = new FileInfo(oggPath[0]);

			List<FileInfo> files = new List<FileInfo>();
			files.Add(descFile);
			files.Add(oggFile);

			//push all .cues files to list
			var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
			if (cueFiles.Length > 0) {
				foreach (var cue in cueFiles) {
					FileInfo file = new FileInfo(cue);
					files.Add(file);
				}
			}

			string path = StandaloneFileBrowser.SaveFilePanel("Audica Save", Application.persistentDataPath, "Edica Save", "edica");
			//PlayerPrefs.SetString("previousSave", path);
			if (path.Length > 0)
				Compress(files, path);

		}

		public void Export() {
			string dirpath = Application.persistentDataPath;

			CueFile export = new CueFile();
			export.cues = new List<Cue>();

			foreach (Target target in orderedNotes) {
				export.cues.Add(NotePosCalc.ToCue(target, offset, false));
			}

			audicaFile.diffs.expert = export;

			AudicaExporter.ExportToAudicaFile(audicaFile);

			NotificationShower.AddNotifToQueue(new NRNotification("Map saved successfully!"));


		}

		public void ExportAndPlay() {
			Export();
			string songFolder = PathLogic.GetSongFolder();
			File.Copy(audicaFile.filepath, Path.Combine(songFolder, audicaFile.desc.songID + ".audica"));

			string newPath = Path.GetFullPath(Path.Combine(songFolder, @"..\..\..\..\"));
			System.Diagnostics.Process.Start(Path.Combine(newPath, "Audica.exe"));
		}

		public void ExportCueToTemp() {
			Cue[] cues = new Cue[notes.Count];
			for (int i = 0; i < notes.Count; i++) {
				//cues[i] = orderedNotes[i].ToCue(offset);
			}

			string json = JsonUtility.ToJson(cues, true);
			json = json.Replace("Items", "cues");

			string dirpath = Application.persistentDataPath;

			//if (notes.Count > 0)
			//File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);
		}

		public void Compress(List<FileInfo> files, string destination) {
			string dirpath = Application.persistentDataPath;

			if (!System.IO.Directory.Exists(dirpath + "\\TempSave\\")) {
				//System.IO.Directory.CreateDirectory(dirpath + "\\TempSave\\", securityRules);

			}

			foreach (FileInfo fileToCompress in files) {
				fileToCompress.CopyTo(dirpath + "\\TempSave\\" + fileToCompress.Name, true);
			}

			try {
				ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
			} catch (IOException e) {
				Debug.Log(e.Message + "....Deleting File");
				FileInfo fileToReplace = new FileInfo(destination);
				fileToReplace.Delete();
				try {
					ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
				} catch (IOException e2) {
					Debug.Log(e2.Message + "....No More attempts");
				}

			}
			DirectoryInfo dir = new DirectoryInfo(dirpath + "\\TempSave\\");
			dir.Delete(true);
		}

		public void NewFile() {
			DeleteAllTargets();

			notes = new List<Target>();
			//notesTimeline = new List<TimelineTarget>();

			string dirpath = Application.persistentDataPath;

			//create the new song desc
			//songDesc = new SongDescyay();

			//locate and copy ogg file to temp folder (used to save the project later)
			var audioFiles = StandaloneFileBrowser.OpenFilePanel("Import .ogg file", System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic), "ogg", false);
			if (audioFiles.Length > 0) {
				FileInfo oggFile = new FileInfo(audioFiles[0]);
				if (!System.IO.Directory.Exists(dirpath + "\\temp\\")) {
					//System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
				} else {
					DirectoryInfo direct = new DirectoryInfo(dirpath + "\\temp\\");
					direct.Delete(true);

					//System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
				}
				oggFile.CopyTo(dirpath + "\\temp\\" + oggFile.Name, true);

				//load ogg into audio clip
				if (audioFiles.Length > 0) {
					StartCoroutine(GetAudioClip(audioFiles[0]));
				} else {
					Debug.Log("ogg not found");
				}
			} else {
				Application.Quit();
			}

		}


		public void LoadTimingMode(AudioClip clip) {

			if (audicaLoaded) {
				return;
			}

			inTimingMode = true;

			aud.clip = clip;
			audioLoaded = true;
		}


		public void SetTimingModeStats(double newBPM, int tickOffset) {
			DeleteAllTargets();
			SetBPM((float)newBPM);

			Cue cue = new Cue();
			cue.pitch = 40;
			cue.tickLength = 1;
			cue.behavior = TargetBehavior.Metronome;
			cue.velocity = TargetVelocity.Metronome;
			cue.handType = TargetHandType.Either;

			for (int i = 0; i < 100; i++) {
				cue.tick = (480 * i) + tickOffset;
				AddTarget(cue);
			}

			//time = 0;
            SafeSetTime();
		}


		public void ExitTimingMode() {

			inTimingMode = false;
			DeleteAllTargets();

		}

		public bool LoadAudicaFile(bool loadRecent = false, string filePath = null) {

			DeleteAllTargets();

			if (loadRecent) {
				audicaFile = AudicaHandler.LoadAudicaFile(PlayerPrefs.GetString("recentFile", null));
				if (audicaFile == null) return false;

			} else if (filePath != null) {
				audicaFile = AudicaHandler.LoadAudicaFile(filePath);

			} else {

				string[] paths = StandaloneFileBrowser.OpenFilePanel("Audica File (Not OST)", Path.Combine(Application.persistentDataPath), "audica", false);

				if (paths.Length == 0) return false;
				audicaFile = AudicaHandler.LoadAudicaFile(paths[0]);
				PlayerPrefs.SetString("recentFile", paths[0]);
			}

			//SetOffset(0);
			desc = audicaFile.desc;


			//Loads all the sounds.
			StartCoroutine(GetAudioClip($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}.ogg"));
			StartCoroutine(LoadLeftSustain($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongLeft}.ogg"));
			StartCoroutine(LoadRightSustain($"file://{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongRight}.ogg"));

			foreach (Cue cue in audicaFile.diffs.expert.cues) {
				AddTarget(cue);
			}
			//LoadTimingMode();

			//Loaded successfully
			return true;

		
			

		}


		IEnumerator GetAudioClip(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					aud.clip = myClip;
					previewAud.clip = myClip;


					SetBPM((float) audicaFile.desc.tempo);
					audioLoaded = true;
					audicaLoaded = true;
					//SetScale(20);
					//Resources.FindObjectsOfTypeAll<OptionsMenu>().First().Init(bpm, offset, beatSnap, songid, songtitle, songartist, songendevent, songpreroll, songauthor);

					//spectrogram.GetComponentInChildren<AudioWaveformVisualizer>().Init();

					curSongName.text = desc.title;
					curSongDiff.text = "Expert";


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
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					leftSustainAud.clip = myClip;
					leftSustainAud.volume = 0f;
				}
			}
		}
		IEnumerator LoadRightSustain(string uri) {
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
				yield return www.SendWebRequest();

				if (www.isNetworkError) {
					Debug.Log(www.error);
				} else {
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					rightSustainAud.clip = myClip;
					rightSustainAud.volume = 0f;

				}
			}
		}


		public void UpdateSongDesc(string songID, string title, int bpm, string songEndEvent = "C#", string mapper = "", int offset = 0) {
			audicaFile.desc.songID = songID;
			audicaFile.desc.title = title;
			audicaFile.desc.tempo = bpm;
			audicaFile.desc.songEndEvent = songEndEvent;
			audicaFile.desc.mapper = mapper;
			audicaFile.desc.offset = offset;
			SetBPM(bpm);
			SetOffset(offset);
		}



		public void SetPlaybackSpeed(float speed) {
			if (!audioLoaded) return;

			playbackSpeed = speed;
			aud.pitch = speed;
			previewAud.pitch = speed;
			leftSustainAud.pitch = speed;
			rightSustainAud.pitch = speed;
		}

		public void SetBPM(float newBpm) {
			bpm = newBpm;
			//TODO:
			//audicaFile.desc.tempo = newBpm;
			SetScale(scale);
		}

		public void SetOffset(int newOffset) {
			offset = newOffset;
			//songDesc.offset = newOffset;
			//audicaFile.desc.offset = newOffset;
		}

		public void SetSnap(int newSnap) {
			beatSnap = newSnap;
		}
		public void BeatSnapChanged() {
			string temp = beatSnapSelector.elements[beatSnapSelector.index];
			int snap = 4;
			int.TryParse(temp.Substring(2), out snap);
			beatSnap = snap;
		}


		public void SetBeatTime(float t) {
			timelineBG.material.SetTextureOffset("_MainTex", new Vector2(((t * bpm / 60 - offset / 480f) / 4f + scaleOffset), 1));

			timelineTransformParent.transform.localPosition = Vector3.left * (t * bpm / 60 - offset / 480f) / (scale / 20f);
			//spectrogram.localPosition = Vector3.left * (t * bpm / 60) / (scale / 20f);

			gridTransformParent.transform.localPosition = Vector3.back * (t * bpm / 60 - offset / 480f);
		}

		public void SetScale(int newScale) {
			if (newScale < 10 || newScale > 35) return;
			timelineBG.material.SetTextureScale("_MainTex", new Vector2(newScale / 4f, 1));
			scaleOffset = -newScale % 8 / 8f;

			//spectrogram.localScale = new Vector3(aud.clip.length / 2 * bpm / 60 / (newScale / 20f), 1, 1);

			timelineTransformParent.transform.localScale *= (float) scale / newScale;
			targetScale *= (float) newScale / scale;
			// fix scaling on all notes
			foreach (Transform note in timelineTransformParent.transform) {
				note.localScale = targetScale * Vector3.one;
			}


			scale = newScale;

			foreach (Target target in orderedNotes) {
				if (target.behavior == TargetBehavior.Hold) {
					//TODO: Fix sustain line scaling when scaling timeline
					//target.timelineTargetIcon.SetSustainLength(target.beatLength);
				}
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

		IEnumerator CalculateNoteCollidersEnabled() {

			int framesToSplitOver = 50;

			int amtToCalc = Mathf.RoundToInt(orderedNotes.Count / framesToSplitOver);

			int j = 0;

			for (int i = 0; i < framesToSplitOver; i++) {

				while(j < orderedNotes.Count) {

					float targetPos = orderedNotes[j].gridTargetIcon.transform.position.z;

					if (targetPos > -20 && targetPos < 20) {
						orderedNotes[j].gridTargetIcon.sphereCollider.enabled = true;
					} else {
						orderedNotes[j].gridTargetIcon.sphereCollider.enabled = false;
					}

					
					if (j > amtToCalc * (i + 1)) break;

					j++;
				}



				yield return null;
				
			}

			while (j < orderedNotes.Count) {
				float targetPos = orderedNotes[j].gridTargetIcon.transform.position.z;

				if (targetPos > -20 && targetPos < 20) {
					orderedNotes[j].gridTargetIcon.sphereCollider.enabled = true;
				} else {
					orderedNotes[j].gridTargetIcon.sphereCollider.enabled = false;
				}
				j++;
			}
			StartCoroutine(CalculateNoteCollidersEnabled());

		}


		public void EnableNearSustainButtons() {
			foreach (Target target in loadedNotes) {
				if (target.behavior !=  TargetBehavior.Hold) continue;
				if (paused && EditorInput.selectedTool == EditorTool.DragSelect && target.gridTargetIcon.transform.position.z < 2 && target.gridTargetIcon.transform.position.z > -2) {
					target.gridTargetIcon.GetComponentInChildren<HoldTargetManager>().EnableSustainButtons();
				} else {
					target.gridTargetIcon.GetComponentInChildren<HoldTargetManager>().DisableSustainButtons();
				}
			}
		}


		
		bool checkForNearSustainsOnThisFrame = false;
		public void Update() {			

			UpdateSustains();

			if (!paused) time += Time.deltaTime * playbackSpeed;

			bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

			if (hover) {
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
					if (Input.mouseScrollDelta.y > 0.1f) {
						SetScale(scale - 1);
					} else if (Input.mouseScrollDelta.y < -0.1f) {
						SetScale(scale + 1);
					}
				}
			}
			//if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && hover))

			if (!isShiftDown && Input.mouseScrollDelta.y < -0.1f) {
				if (!audioLoaded) return;
				time += BeatsToDuration(4f / beatSnap);
				time = SnapTime(time);

				SafeSetTime();
				if (paused) {
					previewAud.Play();
					checkForNearSustainsOnThisFrame = true;
				}


			} else if (!isShiftDown && Input.mouseScrollDelta.y > 0.1f) {
				if (!audioLoaded) return;
				time -= BeatsToDuration(4f / beatSnap);
				time = SnapTime(time);
				SafeSetTime();
				if (paused) {
					previewAud.Play();
					checkForNearSustainsOnThisFrame = true;
				}
				
			}

			SetBeatTime(time);
			if (previewAud.time > time + previewDuration) {
				previewAud.Pause();
			}

			previewAud.volume = aud.volume;

			SetCurrentTime();
			SetCurrentTick();

			miniTimeline.SetPercentagePlayed(GetPercentagePlayed());


			EnableNearSustainButtons();

		}


		public void JumpToPercent(float percent) {
			if (!audioLoaded) return;

			time = SnapTime(aud.clip.length * percent);

			SafeSetTime();
			SetCurrentTime();
			SetCurrentTick();
		}


		public void TogglePlayback() {
			if (!audioLoaded) return;

			if (paused) {
				aud.Play();
				//metro.StartMetronome();

				previewAud.Pause();

				if (leftSustainAud.clip && time < leftSustainAud.clip.length) {
					leftSustainAud.Play();
					rightSustainAud.Play();

				}

				paused = false;
			} else {
				aud.Pause();
				leftSustainAud.Pause();
				rightSustainAud.Pause();
				paused = true;

				//Uncommented
				//time = SnapTime(time);
				if (time < 0) time = 0;
				if (time > aud.clip.length) time = aud.clip.length;

			}
		}

		public void SafeSetTime() {
			if (time < 0) time = 0;
			if (!audioLoaded) return;

			//TODO: Check if song is loaded

			if (time > aud.clip.length) {
				time = aud.clip.length;
			}
			aud.time = time;
			previewAud.time = time;

			float tempTime = time;
			if (leftSustainAud.clip && time > leftSustainAud.clip.length) {
				tempTime = leftSustainAud.clip.length;
			}
			leftSustainAud.time = tempTime;

			if (rightSustainAud.clip && time > rightSustainAud.clip.length) {
				tempTime = rightSustainAud.clip.length;
			}
			rightSustainAud.time = tempTime;
		}

		void OnMouseOver() {
			hover = true;
		}

		void OnMouseExit() {
			hover = false;
		}

		public float GetPercentagePlayed() {
			if (aud.clip != null)
				return (time / aud.clip.length);

			else
				return 0;
		}

		public static float DurationToBeats(float t) {
			return t * bpm / 60;
		}

		public float BeatsToDuration(float beat) {
			return beat * 60 / bpm;
		}

		public float Snap(float beat) {
			return Mathf.Round(beat * beatSnap / 4f) * 4f / beatSnap;
		}

		public static float BeatTime() {
			return DurationToBeats(time) - offset / 480f;
		}

		public float SnapTime(float t) {
			return BeatsToDuration(Snap(DurationToBeats(t) - offset / 480f) + offset / 480f);
		}

		string prevTimeText;
		private void SetCurrentTime() {
			string minutes = Mathf.Floor((int) time / 60).ToString("00");
			string seconds = ((int) time % 60).ToString("00");
			if (seconds != prevTimeText) {
				prevTimeText = seconds;
				songTimestamp.text = minutes + ":" + seconds;
			}

		}

		string prevTickText;
		private void SetCurrentTick() {
			string currentTick = Mathf.Floor((int) BeatTime() * 480f).ToString();
			if (currentTick != prevTickText) {
				prevTickText = currentTick;
				curTick.text = currentTick;
			}

		}

	}
}