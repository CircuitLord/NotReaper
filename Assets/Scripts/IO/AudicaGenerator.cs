using System.Diagnostics;
using System.IO;
using System.Text;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using NotReaper.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common.Zip;
using SharpCompress.Writers;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaGenerator {

		public static string Generate(string oggPath, string songID, string songName, string artist, double bpm, string songEndEvent, string mapper, int offset) {

			var streamAssets = Application.streamingAssetsPath;
			var ogg2AudicaFolder = Path.Combine(streamAssets, "Ogg2Audica");

			/*
			var audicaTemplate = Path.Combine(streamAssets, ".AudicaTemplate/");

			Encoding encoding = Encoding.GetEncoding("UTF-8");


			//We need to modify the BPM of the song.mid contained in the template audica to match whatever this is.
			File.Delete(Path.Combine(streamAssets, "song.mid"));
			MidiFile songMidi = MidiFile.Read(Path.Combine(streamAssets, "songtemplate.mid"));

			songMidi.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(bpm)));
			songMidi.Write(Path.Combine(streamAssets, "song.mid"), true, MidiFileFormat.MultiTrack);


			//Generates the mogg into song.mogg, which is moved to the .AudicaTemplate
			File.Delete(Path.Combine(streamAssets, "song.mogg"));

			System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			startInfo.FileName = Path.Combine(streamAssets, "ogg2mogg.exe");
			string args = $"\"{oggPath}\" \"{streamAssets}/song.mogg\"";
			startInfo.Arguments = args;
			startInfo.UseShellExecute = false;

			myProcess.StartInfo = startInfo;
			myProcess.Start();

			myProcess.WaitForExit();


			//Make the song.desc file;
			File.Delete(Path.Combine(streamAssets, "song.desc"));
			SongDesc songDesc = JsonUtility.FromJson<SongDesc>(File.ReadAllText(Path.Combine(streamAssets, "songtemplate.desc")));
			songDesc.songID = songID;
			songDesc.title = songName;
			songDesc.artist = artist;
			songDesc.tempo = bpm;
			songDesc.songEndEvent = songEndEvent;
			songDesc.mapper = mapper;
			songDesc.offset = offset;
			File.WriteAllText(Path.Combine(streamAssets, "song.desc"), JsonUtility.ToJson(songDesc, true));


			

			/*
			ZipArchive zip = ZipArchive.Open(Path.Combine(streamAssets, "AudicaTemplate.zip"));

			zip.AddAllFromDirectory(audicaTemplate);
			zip.AddEntry("song.desc", Path.Combine(streamAssets, "song.desc"));
			zip.AddEntry("song.mid", Path.Combine(streamAssets, "song.mid"));
			zip.AddEntry("song.mogg", Path.Combine(streamAssets, "song.mogg"));

			zip.SaveTo(Path.Combine(Application.persistentDataPath, "saves", songID + ".audica"), SharpCompress.Common.CompressionType.None);
			
			zip.Dispose();
			

	

			using(ZipArchive archive = ZipArchive.Create()) {
				archive.AddAllFromDirectory(audicaTemplate);
				archive.AddEntry("song.desc", Path.Combine(streamAssets, "song.desc"));
				archive.AddEntry("song.mid", Path.Combine(streamAssets, "song.mid"));
				archive.AddEntry("song.mogg", Path.Combine(streamAssets, "song.mogg"));
				archive.SaveTo(Path.Combine(Application.persistentDataPath, "saves", songID + ".audica"), SharpCompress.Common.CompressionType.None);
			}

			*/
		
		
			HandleCache.CheckSaveFolderValid();

			System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			startInfo.FileName = Path.Combine(ogg2AudicaFolder, "Ogg2Audica.exe");

			startInfo.Arguments = System.String.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" \"{6}\" \"{7}\"", oggPath, songID, songName, artist, bpm, songEndEvent, mapper, offset);
			startInfo.UseShellExecute = true;
			startInfo.WorkingDirectory = ogg2AudicaFolder;

			myProcess.StartInfo = startInfo;
			
			myProcess.Start();

			myProcess.WaitForExit();

			File.Move(Path.Combine(ogg2AudicaFolder, "out.audica"), Path.Combine(Application.dataPath, "saves", songID + ".audica"));


			return Path.Combine(Application.dataPath, "saves", songID + ".audica");


		}

	}

}