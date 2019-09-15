using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using Newtonsoft.Json;
using NotReaper.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using UnityEngine;

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

				bool expert = false, advanced = false, standard = false, easy = false;
				//Write the cues files to disk so we can add them to the audica file.
				if (audicaFile.diffs.expert.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/expert-new.cues", CuesToJson(audicaFile.diffs.expert));
					expert = true;
				}
				if (audicaFile.diffs.advanced.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/advanced-new.cues", CuesToJson(audicaFile.diffs.advanced));
					advanced = true;
				}
				if (audicaFile.diffs.standard.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/standard-new.cues", CuesToJson(audicaFile.diffs.standard));
					standard = true;
				}
				if (audicaFile.diffs.easy.cues != null) {
					File.WriteAllText($"{Application.dataPath}/.cache/easy-new.cues", CuesToJson(audicaFile.diffs.easy));
					easy = true;
				}

				File.WriteAllText($"{Application.dataPath}/.cache/song-new.desc", JsonUtility.ToJson(audicaFile.desc));

				//Remove any files we'll be replacing
				foreach (ZipArchiveEntry entry in archive.Entries) {

					if (entry.ToString() == "expert.cues") {
						archive.RemoveEntry(entry);
						
					} else if (entry.ToString() == "song.desc") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "advanced.cues") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "standard.cues") {
						archive.RemoveEntry(entry);
					} else if (entry.ToString() == "easy.cues") {
						archive.RemoveEntry(entry);
					}
					

				}
				if (expert) archive.AddEntry("expert.cues", $"{Application.dataPath}/.cache/expert-new.cues");
				if (advanced) archive.AddEntry("advanced.cues", $"{Application.dataPath}/.cache/advanced-new.cues");
				if (standard) archive.AddEntry("standard.cues", $"{Application.dataPath}/.cache/standard-new.cues");
				if (easy) archive.AddEntry("easy.cues", $"{Application.dataPath}/.cache/easy-new.cues");

				archive.AddEntry("song.desc", $"{Application.dataPath}/.cache/song-new.desc");
				archive.SaveTo(audicaFile.filepath + ".temp", SharpCompress.Common.CompressionType.None);
				archive.Dispose();



			}
			File.Delete(audicaFile.filepath);
			File.Move(audicaFile.filepath + ".temp", audicaFile.filepath);


			Debug.Log("Export finished.");


		}


		public static string CuesToJson(CueFile cueFile) {
			return JsonUtility.ToJson(cueFile, true);
		}


	}

}