using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using NAudio.Midi;
using Newtonsoft.Json;
using NotReaper.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using UnityEngine;
using NotReaper.Timing;
using NotReaper.Modifier;

namespace NotReaper.IO {

	public class AudicaExporter {

		
		public static void ExportToAudicaFile(AudicaFile audicaFile) {

			if (!File.Exists(audicaFile.filepath)) {
				Debug.Log("Save file is gone... :(");
				return;
			}

			Encoding encoding = Encoding.GetEncoding(437);

			using(var archive = ZipArchive.Open(audicaFile.filepath)) {


				HandleCache.CheckCacheFolderValid();
				HandleCache.CheckSaveFolderValid();

                bool expert = false, advanced = false, standard = false, easy = false, modifiers = false;
				//Write the cues files to disk so we can add them to the audica file.
				if (audicaFile.diffs.expert.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/expert-new.cues", CuesToJson(audicaFile.diffs.expert));
					expert = true;
				}
				if (audicaFile.diffs.advanced.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/advanced-new.cues", CuesToJson(audicaFile.diffs.advanced));
					advanced = true;
				}
				if (audicaFile.diffs.moderate.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/moderate-new.cues", CuesToJson(audicaFile.diffs.moderate));
					standard = true;
				}
				if (audicaFile.diffs.beginner.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/beginner-new.cues", CuesToJson(audicaFile.diffs.beginner));
					easy = true;
				}
                audicaFile.modifiers.modifiers = ModifierHandler.Instance.MapToDTO();
                if (audicaFile.modifiers.modifiers.Count > 0)
                {
                    //File.WriteAllText($"{Application.dataPath}/.cache/modifiers-new.json", ModifiersToJson(audicaFile.modifiers));
                    File.WriteAllText($"{Application.dataPath}/.cache/modifiers-new.json", ModifiersToJson2(audicaFile.modifiers));
                    modifiers = true;
                }

				File.WriteAllText($"{Application.dataPath}/.cache/{audicaFile.desc.moggSong}", audicaFile.mainMoggSong.ExportToText());
				File.WriteAllText($"{Application.dataPath}/.cache/song-new.desc", JsonUtility.ToJson(audicaFile.desc));

				var workFolder = Path.Combine(Application.streamingAssetsPath, "Ogg2Audica");
				MidiFile songMidi = new MidiFile(Path.Combine(workFolder, "songtemplate.mid"));

				MidiEventCollection events = new MidiEventCollection(0, (int)Constants.PulsesPerQuarterNote);
				foreach(var tempo in audicaFile.desc.tempoList) {
					events.AddEvent(new TempoEvent((int)tempo.microsecondsPerQuarterNote, (long)tempo.time.tick), 0);
					events.AddEvent(new TimeSignatureEvent((long)tempo.time.tick, (int)tempo.timeSignature.Numerator, (int)TimeSignature.GetMIDIDenominator(tempo.timeSignature.Denominator), 0, 8), 0);
				}

				events.PrepareForExport();
				MidiFile.Export(Path.Combine(workFolder, $"{Application.dataPath}/.cache/song.mid"), events);

				//Remove any files we'll be replacing
				foreach (ZipArchiveEntry entry in archive.Entries) {

					if (entry.ToString() == "expert.cues") {
						archive.RemoveEntry(entry);
						


					} else if (entry.ToString() == "song.desc") {
						archive.RemoveEntry(entry);
					}
					else if (entry.ToString() == audicaFile.desc.moggSong)
					{	archive.RemoveEntry(entry);
					} else if (entry.ToString() == "song.mid") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "song.png") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "advanced.cues") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "moderate.cues") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "beginner.cues") {
						archive.RemoveEntry(entry);
					} else if(entry.ToString() == "modifiers.json")
                    {
                        archive.RemoveEntry(entry);
                    }
					

				}
				if (expert) archive.AddEntry("expert.cues", $"{Application.dataPath}/.cache/expert-new.cues");
				if (advanced) archive.AddEntry("advanced.cues", $"{Application.dataPath}/.cache/advanced-new.cues");
				if (standard) archive.AddEntry("moderate.cues", $"{Application.dataPath}/.cache/moderate-new.cues");
				if (easy) archive.AddEntry("beginner.cues", $"{Application.dataPath}/.cache/beginner-new.cues");
                if (modifiers) archive.AddEntry("modifiers.json", $"{Application.dataPath}/.cache/modifiers-new.json");

				archive.AddEntry($"{audicaFile.desc.moggSong}", $"{Application.dataPath}/.cache/{audicaFile.desc.moggSong}");
				archive.AddEntry("song.desc", $"{Application.dataPath}/.cache/song-new.desc");
				archive.AddEntry("song.mid", $"{Application.dataPath}/.cache/song.mid");
				if (File.Exists($"{Application.dataPath}/.cache/song.png"))
					{
					archive.AddEntry("song.png", $"{Application.dataPath}/.cache/song.png");
				}
				archive.SaveTo(audicaFile.filepath + ".temp", SharpCompress.Common.CompressionType.None);
				archive.Dispose();



			}
			File.Delete($"{Application.dataPath}/.cache/{audicaFile.desc.moggSong}");
			
			File.Delete(audicaFile.filepath);
			File.Move(audicaFile.filepath + ".temp", audicaFile.filepath);


			Debug.Log("Export finished.");


		}


		public static string CuesToJson(CueFile cueFile) {
			return JsonUtility.ToJson(cueFile, true);
		}

        public static string ModifiersToJson2(ModifierList modifiers)
        {
            return JsonUtility.ToJson(modifiers, true);
        }

        public static string ModifiersToJson(ModifierList modifiers)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            return JsonConvert.SerializeObject(modifiers, Formatting.Indented, settings);
        }

	}

}