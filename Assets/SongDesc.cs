public class SongDesc
{
    public string songID;
    public string moggSong;
    public string title;
    public string artist;
    public string midiFile;
    public string fusionSpatialized;
    public string sustainSongRight;
    public string sustainSongLeft;
    public string fxSong;
    public float tempo = 128.0f; // bpm
    public string songEndEvent;
    public float prerollSeconds = 0;
    public bool useMidiForCues = false;
    public bool hidden = false;
    public int offset = 0; // offset
}
