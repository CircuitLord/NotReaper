using System.IO;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common.Zip;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaGenerator {

		public void Generate(string oggPath, string songName, int bpm) {

			//Template files are in here. in .AudicaTemplate/
			var streamAssets = Application.streamingAssetsPath;
			var audicaTemplate = Path.Combine(streamAssets, ".AudicaTemplate/");


			//We need to modify the BPM of the song.mid contained in the template audica to match whatever this is.
			MidiFile songMidi = MidiFile.Read(Path.Combine(audicaTemplate, "song.mid"));

			songMidi.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(bpm)));

			songMidi.Write(Path.Combine(audicaTemplate, "song.mid"), true, MidiFileFormat.MultiTrack);


			HandleCache.CheckSaveFolderValid();

			using(ZipArchive archive = ZipArchive.Create()) {
				archive.AddAllFromDirectory(audicaTemplate);
				archive.SaveTo(Path.Combine(Application.persistentDataPath, "saves", songName), SharpCompress.Common.CompressionType.None);
			}


		}

	}

}