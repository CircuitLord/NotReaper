using System;
using System.Collections;
using System.IO;
using System.Text;
using Ionic.Zip;
using Newtonsoft.Json;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.Networking;

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


			CheckCacheValid();
			//Remove any existing cue files.
			ClearCueCache();

			//Figure out what files we need to extract by getting the song.desc.
			foreach (ZipEntry entry in audicaZip.Entries) {
				if (entry.FileName == "song.desc") {
					MemoryStream ms = new MemoryStream();
					entry.Extract(ms);
					string tempDesc = Encoding.UTF8.GetString(ms.ToArray());

					audicaFile.desc = JsonUtility.FromJson<SongDesc>(tempDesc);
					audicaFile.safeDesc.songID = audicaFile.desc.songID;
					audicaFile.safeDesc.moggSong = audicaFile.desc.moggSong;
					audicaFile.safeDesc.title = audicaFile.desc.title;
					audicaFile.safeDesc.artist = audicaFile.desc.artist;
					audicaFile.safeDesc.midiFile = audicaFile.desc.midiFile;
					audicaFile.safeDesc.fusionSpatialized = audicaFile.desc.fusionSpatialized;
					audicaFile.safeDesc.fusionUnspatialized = audicaFile.desc.fusionUnspatialized;
					audicaFile.safeDesc.sustainSongRight = audicaFile.desc.sustainSongRight;
					audicaFile.safeDesc.sustainSongLeft = audicaFile.desc.sustainSongLeft;
					audicaFile.safeDesc.fxSong = audicaFile.desc.fxSong;
					audicaFile.safeDesc.tempo = audicaFile.desc.tempo;
					audicaFile.safeDesc.songEndEvent = audicaFile.desc.songEndEvent;
					audicaFile.safeDesc.prerollSeconds = audicaFile.desc.prerollSeconds;
					audicaFile.safeDesc.useMidiForCues = audicaFile.desc.useMidiForCues;
					audicaFile.safeDesc.hidden = audicaFile.desc.hidden;
					audicaFile.safeDesc.offset = audicaFile.desc.offset;
					audicaFile.safeDesc.mapper = audicaFile.desc.mapper;

					ms.Dispose();
					continue;
				}

				//Extract the cues files.
				else if (entry.FileName == $"{CuesDifficulty.expert}.cues") {
					entry.Extract($"{appPath}/.cache");
					expert = true;

				} else if (entry.FileName == $"{CuesDifficulty.advanced}.cues") {
					entry.Extract($"{appPath}/.cache");
					advanced = true;

				} else if (entry.FileName == $"{CuesDifficulty.standard}.cues") {
					entry.Extract($"{appPath}/.cache");
					standard = true;

				} else if (entry.FileName == $"{CuesDifficulty.easy}.cues") {
					entry.Extract($"{appPath}/.cache");
					easy = true;
				}

			}

			//Now we fill the audicaFile var with all the things it needs.
			//Remember, all props in audicaFile.desc refer to either moggsong or the name of the mogg.
			//Real clips are stored in main audicaFile object.


			//Load the cues files.
			if (expert) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.expert}.cues"));
			}
			if (advanced) {
				audicaFile.diffs.advanced = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.advanced}.cues"));
			}
			if (standard) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.standard}.cues"));
			}
			if (easy) {
				audicaFile.diffs.expert = JsonUtility.FromJson<CueFile>(File.ReadAllText($"{appPath}/.cache/{CuesDifficulty.easy}.cues"));
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


			bool mainSongLoaded = false;
			bool sustainRightLoaded = false;
			bool sustainLeftLoaded = false;

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}.ogg")) {
				//audicaFile.song = (AudioClip) Resources.Load($"{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}", typeof(AudioClip));


				//var clip1 = www.GetAudioClip(true, true);
				//audicaFile.song = clip1;

				mainSongLoaded = true;
			}

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongRight}.ogg")) {
				//audicaFile.song_sustain_r = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongRight}");
				sustainRightLoaded = true;
			}

			if (File.Exists($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongLeft}.ogg")) {
				//audicaFile.song_sustain_l = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongLeft}");
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
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedMainSong);
					audicaFile.song = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedMainSong}");
					mainSongLoaded = true;

				} else if (!sustainRightLoaded && entry.FileName == audicaFile.desc.moggSustainSongRight) {
					entry.Extract(tempMogg);
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedSustainSongRight);
					audicaFile.song_sustain_r = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongRight}");
					sustainRightLoaded = true;

				} else if (!sustainLeftLoaded && entry.FileName == audicaFile.desc.moggSustainSongLeft) {
					entry.Extract(tempMogg);
					MoggToOgg(tempMogg.ToArray(), audicaFile.desc.cachedSustainSongLeft);
					audicaFile.song_sustain_l = Resources.Load<AudioClip>($"{Application.dataPath}/.cache/{audicaFile.desc.cachedSustainSongLeft}");
					sustainLeftLoaded = true;

				}

				tempMogg.SetLength(0);

			}

			Finish:

				audicaFile.filepath = path;
			audicaZip.Dispose();

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
			File.WriteAllBytes($"{Application.dataPath}/.cache/{filename}.ogg", dst);

		}


		public static void CheckCacheValid() {
			if (!Directory.Exists($"{Application.dataPath}/.cache")) {
				Directory.CreateDirectory($"{Application.dataPath}/.cache");
			}
		}
		//TODO: Make use varaibles for names
		public static void ClearCueCache() {
			File.Delete($"{Application.dataPath}/.cache/expert.cues");
			File.Delete($"{Application.dataPath}/.cache/advanced.cues");
			File.Delete($"{Application.dataPath}/.cache/standard.cues");
			File.Delete($"{Application.dataPath}/.cache/easy.cues");
		}

	}
}