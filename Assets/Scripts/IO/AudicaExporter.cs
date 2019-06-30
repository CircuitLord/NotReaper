using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaExporter : MonoBehaviour {

		public void ExportToAudicaFile(AudicaFile audicaFile) {

			if (!File.Exists(audicaFile.filepath)) {

			}

			ZipFile audicaZip = ZipFile.Read(audicaFile.filepath);


			AudicaHandler.CheckCacheValid();

			//Write the cues files to disk so we can add them to the audica file.
			if (audicaFile.diffs.expert.cues != null) {
				File.WriteAllText($"{Application.dataPath}/.cache/expert-new.cues", CuesToJson(audicaFile.diffs.expert));
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


			//Cache the song desc.
			File.WriteAllText($"{Application.dataPath}/.cache/song.desc", JsonConvert.SerializeObject(audicaFile.desc));
			Debug.Log("Import and export finished.");

		}


		class TempCues {
			public List<Cue> cues;
		}

		public string CuesToJson(CueFile cueFile) {
			return JsonConvert.SerializeObject(cueFile);
		}


		private void Update() {
			if (Input.GetKeyDown(KeyCode.L)) {
				AudicaFile audicaFile = AudicaHandler.LoadAudicaFile(@"C:\Files\GameStuff\AUDICACustom\Testing\test.audica");
				ExportToAudicaFile(audicaFile);
			}
		}


	}

}