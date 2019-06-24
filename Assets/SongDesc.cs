public class SongDesc
{
    public string songID;
    public string moggSong = "song.moggsong";
    public string title;
    public string artist;
    public string midiFile = "song.mid";
    public string fusionSpatialized = "fusion/guns/default/drums_default_spatial.fusion";
    public string fusionUnspatialized = "fusion/guns/default/drums_default_sub.fusion";
    public string sustainSongRight = "song_sustain_r.moggsong";
    public string sustainSongLeft = "song_sustain_l.moggsong";
    public string fxSong = "song_extras.moggsong";
    public float tempo = 128.0f; // bpm
    public string songEndEvent = "event:/song_end/song_end_C#";
    public float prerollSeconds = 0.0f;
    public bool useMidiForCues = false;
    public bool hidden = false;
    public int offset = 0; // offset
    public string author; // author
}
