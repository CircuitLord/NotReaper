using System;
using System.IO;
using System.Text;
using Ionic.Zip;
using Newtonsoft.Json;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.IO {

	public class AudicaHandler : MonoBehaviour {

		public static AudicaFile LoadAudicaFile(string path) {

			AudicaFile audicaFile = new AudicaFile();

			ZipFile audicaZip = ZipFile.Read(path);

			string appPath = Application.dataPath;
			bool easy = false;
			bool standard = false;
			bool advanced = false;
			bool expert = false;


			//Figure out what files we need to extract by getting the song.desc.
			foreach (ZipEntry entry in audicaZip.Entries) {
				if (entry.FileName == "song.desc") {
					MemoryStream ms = new MemoryStream();
					entry.Extract(ms);
					string tempDesc = Encoding.UTF8.GetString(ms.ToArray());

					audicaFile.desc = JsonConvert.DeserializeObject<SongDesc>(tempDesc);

					ms.Dispose();
					continue;
				}

				//Extract the cues files.
				else if (entry.FileName == $"{CuesDifficulty.expert}.cues") {
					entry.Extract($"{appPath}/.cache");

				} else if (entry.FileName == $"{CuesDifficulty.advanced}.cues") {
					entry.Extract($"{appPath}/.cache");

				} else if (entry.FileName == $"{CuesDifficulty.standard}.cues") {
					entry.Extract($"{appPath}/.cache");

				} else if (entry.FileName == $"{CuesDifficulty.easy}.cues") {
					entry.Extract($"{appPath}/.cache");
				}

			}

			//Now we fill the audicaFile var with all the things it needs.
			//Remember, all props in audicaFile.desc refer to either moggsong or the name of the mogg.
			//Real clips are stored in main audicaFile object.


			//Load the cues files.
			if (expert) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.expert}.json"));
			}
			if (advanced) {
				audicaFile.diffs.advanced = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.advanced}.json"));
			}
			if (standard) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.standard}.json"));
			}
			if (easy) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.easy}.json"));
			}

			MemoryStream temp = new MemoryStream();

			//Load the names of the moggs
			foreach (ZipEntry entry in audicaZip.Entries) {

				if (entry.FileName == audicaFile.desc.moggSong) {
					entry.Extract(temp);
					audicaFile.desc.moggMainSong = MoggSongParser.parse_metadata(Encoding.UTF8.GetString(temp.ToArray())) [0];

				} else if (entry.FileName == audicaFile.desc.sustainSongLeft) {
					entry.Extract(temp);
					audicaFile.desc.moggSustainSongLeft = MoggSongParser.parse_metadata(Encoding.UTF8.GetString(temp.ToArray())) [0];

				} else if (entry.FileName == audicaFile.desc.sustainSongRight) {
					entry.Extract(temp);
					audicaFile.desc.moggSustainSongRight = MoggSongParser.parse_metadata(Encoding.UTF8.GetString(temp.ToArray())) [0];
				}

				temp.SetLength(0);

			}

			//Check if we have the moggs cached.
			//If so, load them.
			//If not, cache them then load them.

			//Ensure cache folder exists. If not, it skips over them and goes to caching.
			CheckCacheValid();

			bool mainSongLoaded = false;
			bool sustainRightLoaded = false;
			bool sustainLeftLoaded = false;

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggMainSong}")) {
				audicaFile.song = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggMainSong}");
				mainSongLoaded = true;
			}

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongRight}")) {
				audicaFile.song_sustain_r = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongRight}");
				sustainRightLoaded = true;
			}

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongLeft}")) {
				audicaFile.song_sustain_l = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongLeft}");
				sustainLeftLoaded = true;
			}

			//If all the songs were already cached, skip this and go to the finish.
			if (mainSongLoaded && sustainRightLoaded && sustainLeftLoaded) {
				print("Files were already cached and have been loaded.");
				goto Finish;
			}
			print("Files not cached... Loading...");
			//If the files weren't cached, we now need to cache them manually then load them.
			MemoryStream tempMogg = new MemoryStream();

			foreach (ZipEntry entry in audicaZip.Entries) {

				if (!mainSongLoaded && entry.FileName == audicaFile.desc.moggMainSong) {
					entry.Extract(tempMogg);
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedOggMainSong);
					audicaFile.song = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggMainSong}");
					mainSongLoaded = true;

				} else if (!sustainRightLoaded && entry.FileName == audicaFile.desc.moggSustainSongRight) {
					entry.Extract(tempMogg);
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedOggSustainSongRight);
					audicaFile.song_sustain_r = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongRight}");
					sustainRightLoaded = true;

				} else if (!sustainLeftLoaded && entry.FileName == audicaFile.desc.moggSustainSongLeft) {
					entry.Extract(tempMogg);
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedOggSustainSongLeft);
					audicaFile.song_sustain_l = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedOggSustainSongLeft}");
					sustainLeftLoaded = true;

				}

				tempMogg.SetLength(0);

			}

			Finish:

				audicaFile.filepath = path;

			return audicaFile;
		}

		public static void MoggToOgg(byte[] bytes, string filename) {

			byte[] oggStartLocation = new byte[4];

			oggStartLocation[0] = bytes[4];
			oggStartLocation[1] = bytes[5];
			oggStartLocation[2] = bytes[6];
			oggStartLocation[3] = bytes[7];

			int start = BitConverter.ToInt32(oggStartLocation, 0);

			byte[] dst = new byte[bytes.Length - start];
			Array.Copy(bytes, start, dst, 0, dst.Length);
			File.WriteAllBytes($"{Application.dataPath}/.cache/{filename}", dst);

		}


		public static void CheckCacheValid() {
			if (!Directory.Exists($"{Application.dataPath}/.cache")) {
				Directory.CreateDirectory($"{Application.dataPath}/.cache");
			}
		}

		public void Update() {

			if (Input.GetKeyDown(KeyCode.K)) {


			}
		}

	}
}