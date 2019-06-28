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
			if (audicaFile.cues.expert != null) {
				File.WriteAllText($"{Application.dataPath}/CACHE/expert.cues", CuesToJson(audicaFile.cues.expert));
			}
			if (audicaFile.cues.advanced != null) {
				File.WriteAllText($"{Application.dataPath}/CACHE/advanced.cues", CuesToJson(audicaFile.cues.advanced));
			}
			if (audicaFile.cues.standard != null) {
				File.WriteAllText($"{Application.dataPath}/CACHE/standard.cues", CuesToJson(audicaFile.cues.standard));
			}
			if (audicaFile.cues.easy != null) {
				File.WriteAllText($"{Application.dataPath}/CACHE/easy.cues", CuesToJson(audicaFile.cues.easy));
			}


			//Cache the song desc.
			File.WriteAllText($"{Application.dataPath}/CACHE/song.desc", JsonConvert.SerializeObject(audicaFile.desc));
			print("Import and export finished.");

		}


		class TempCues {
			public List<Cue> cues;
		}

		public string CuesToJson(List<Cue> cues) {
			TempCues tempCues = new TempCues();
			tempCues.cues = cues;
			return JsonConvert.SerializeObject(tempCues);

		}


		private void Update() {
			if (Input.GetKeyDown(KeyCode.L)) {
				AudicaFile audicaFile = AudicaHandler.LoadAudicaFile(@"C:\Files\GameStuff\AUDICACustom\Testing\test.audica");
				ExportToAudicaFile(audicaFile);
			}
		}


	}

}