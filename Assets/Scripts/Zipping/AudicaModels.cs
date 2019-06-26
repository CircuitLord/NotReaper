using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudicaModels {

	//KEEP IN MIND, almost all the song references are to the moggsong, not the mogg.
	// moggSusatinSongRight references the name of the mogg, the actual file is stored on the AudicaFile object.
	public class SongDesc {
		public string songID;

		public string moggSong;
		public string moggMainSong;
		public string cachedOggMainSong {
			get {
				return $"{this.songID}.ogg";
			}
		}

		public string title;
		public string artist;

		public string midiFile;

		public string fusionSpatialized;

		public string sustainSongRight;
		public string moggSustainSongRight;
		public string cachedOggSustainSongRight {
			get {
				return $"{this.songID}_sustain_r.ogg";
			}
		}

		public string sustainSongLeft;
		public string moggSustainSongLeft;
		public string cachedOggSustainSongLeft {
			get {
				return $"{this.songID}_sustain_l.ogg";
			}
		}

		public string fxSong;
		public string moggFxSong;

		public double tempo;
		public string songEndEvent;
		public double prerollSeconds;
		public bool useMidiForCues = false;
		public bool hidden = false;
		public string mapper;
	}


	//TODO: Move cues def to separate file?
	//TODO: Populate Cues class
	public class Cues {

	}

	public class CuesList {
		public Cues expert;
		public Cues advanced;
		public Cues standard;
		public Cues easy;

	}


	public class AudicaFile {
		public SongDesc desc;
		public AudioClip song;
		public CuesList cues;
		public AudioClip song_extras;
		public AudioClip song_sustain_l;
		public AudioClip song_sustain_r;

	}


}
