using System.Diagnostics;
using System.IO;
using System.Text;
using NAudio.Midi;
using NotReaper.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common.Zip;
using SharpCompress.Writers;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaGenerator {

		public static string Generate(string oggPath, string songID, string songName, string artist, double bpm, string songEndEvent, string author, int offset) {


			HandleCache.CheckSaveFolderValid();

			var workFolder = Path.Combine(Application.streamingAssetsPath, "Ogg2Audica");

			
			string audicaTemplate = Path.Combine(workFolder, ".AudicaTemplate/");

			Encoding encoding = Encoding.GetEncoding("UTF-8");


			//We need to modify the BPM of the song.mid contained in the template audica to match whatever this is.
			File.Delete(Path.Combine(workFolder, "song.mid"));
			File.Copy(Path.Combine(workFolder, "songtemplate.mid"), Path.Combine(workFolder, "song.mid"));
			
			//Generates the mogg into song.mogg, which is moved to the .AudicaTemplate
			File.Delete(Path.Combine(workFolder, "song.mogg"));

			Process ogg2mogg = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			startInfo.FileName = Path.Combine(workFolder, "ogg2mogg.exe");
			
			string args = $"\"{oggPath}\" \"{workFolder}/song.mogg\"";
			startInfo.Arguments = args;
			startInfo.UseShellExecute = false;

			ogg2mogg.StartInfo = startInfo;
			ogg2mogg.Start();

			ogg2mogg.WaitForExit();


			//Make the song.desc file;
			File.Delete(Path.Combine(workFolder, "song.desc"));
			SongDesc songDesc = JsonUtility.FromJson<SongDesc>(File.ReadAllText(Path.Combine(workFolder, "songtemplate.desc")));
			songDesc.songID = songID;
			songDesc.title = songName;
			songDesc.artist = artist;
			songDesc.tempo = (float)bpm;
			songDesc.songEndEvent = songEndEvent;
			songDesc.author = author;
			songDesc.offset = offset;
			File.WriteAllText(Path.Combine(workFolder, "song.desc"), JsonUtility.ToJson(songDesc, true));

			
			//Create the actual audica file and save it to the /saves/ folder
			using(ZipArchive archive = ZipArchive.Create()) {
				archive.AddAllFromDirectory(audicaTemplate);
				archive.AddEntry("song.desc", Path.Combine(workFolder, "song.desc"));
				archive.AddEntry("song.mid", Path.Combine(workFolder, "song.mid"));
				archive.AddEntry("song.mogg", Path.Combine(workFolder, "song.mogg"));
				
				
				archive.SaveTo(Path.Combine(Application.dataPath, @"../", "saves", songID + ".audica"), SharpCompress.Common.CompressionType.None);
			}

			return Path.Combine(Application.dataPath, @"../", "saves", songID + ".audica");
			
		/*
		
			HandleCache.CheckSaveFolderValid(); 59.6, 57.8

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
			*/


		}

	}

}