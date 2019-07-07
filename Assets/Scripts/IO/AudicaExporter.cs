using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using NotReaper.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaExporter : MonoBehaviour {

		public static void ExportToAudicaFile(AudicaFile audicaFile) {

			if (!File.Exists(audicaFile.filepath)) {
				Debug.Log("Save file is gone... :(");
				return;
			}


			using(var archive = ZipArchive.Open(audicaFile.filepath)) {


				AudicaHandler.CheckCacheValid();

				//Write the cues files to disk so we can add them to the audica file.
				if (audicaFile.diffs.expert.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/expert.cues", CuesToJson(audicaFile.diffs.expert));
				}
				if (audicaFile.diffs.advanced.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/advanced-new.cues", CuesToJson(audicaFile.diffs.advanced));
				}
				if (audicaFile.diffs.standard.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/standard-new.cues", CuesToJson(audicaFile.diffs.standard));
				}
				if (audicaFile.diffs.easy.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/easy-new.cues", CuesToJson(audicaFile.diffs.easy));
				}

				foreach (ZipArchiveEntry entry in archive.Entries) {

					if (entry.ToString() == "expert.cues") {
						archive.RemoveEntry(entry);
					}

				}

				//archive.RemoveEntry()
				archive.AddEntry("expert.cues", $"{Application.dataPath}/.cache/expert.cues");
				archive.SaveTo(audicaFile.filepath + ".temp", SharpCompress.Common.CompressionType.None);
				archive.Dispose();

				File.Delete(audicaFile.filepath);
				File.Move(audicaFile.filepath + ".temp", audicaFile.filepath);
			}


			Debug.Log("Export finished.");


		}


		class TempCues {
			public List<Cue> cues;
		}

		public static string CuesToJson(CueFile cueFile) {
			return JsonUtility.ToJson(cueFile, true);
		}


		//private void Update() {
		//if (Input.GetKeyDown(KeyCode.L)) {
		//AudicaFile audicaFile = AudicaHandler.LoadAudicaFile(@"C:\Files\GameStuff\AUDICACustom\Testing\test.audica");
		//ExportToAudicaFile(audicaFile);
		//}
		//}


	}

}