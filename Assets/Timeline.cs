using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Timeline : MonoBehaviour {

    public float playbackSpeed = 1f;
    
    [SerializeField] private Renderer timelineBG;
    [SerializeField] private Transform timelineNotes;
    [SerializeField] private Transform gridNotes;
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioSource previewAud;
    [SerializeField] private Transform spectrogram;
    [SerializeField] private TextMeshPro curMinutes;
    [SerializeField] private TextMeshPro curSeconds;
    [SerializeField] private TextMeshPro curTick;

    public Color leftColor;
    public Color rightColor;
    public Color bothColor;
    public Color neitherColor;


    List<GridTarget> notes;
    SongDesc songDesc;

    public GridTarget gridNotePrefab;
    public TimelineTarget timelineNotePrefab;

    public float previewDuration = 0.1f;
    
    public float time { get; private set; }

    private int beatSnap = 4;
    private int scale = 20;
    private float targetScale = 1f;
    private float scaleOffset = 0;
    private float bpm = 60;
    private int offset = 0;
    // .Desc menu
    private string songid = "";
    private string songtitle = "";
    private string songartist = "";
    private string songendevent = "";
    private float songpreroll = 0.0f;
    private string songauthor = "";

    private TargetHandType selectedHandType = TargetHandType.Right;
    private TargetBehavior selectedBehaviour = TargetBehavior.Standard;
    private TargetVelocity selectedVelocity = TargetVelocity.Standard;

    private float timelineMaterialOffset;

    private bool hover = false;
    private bool paused = true;

    
    private void Awake()
    {
        notes = new List<GridTarget>();

        Import();
    }

    public void AddTarget(Cue cue)
    {
        float x = 0, y = 0;

        if (cue.behavior == TargetBehavior.Melee)
        {
            switch (cue.pitch)
            {
                case 98:
                    x = -2f;
                    y = -1;
                    break;
                case 99:
                    x = 2f;
                    y = -1;
                    break;
                case 100:
                    x = -2f;
                    y = 1;
                    break;
                case 101:
                    x = 2f;
                    y = 1;
                    break;

            }
        }
        else
        {
            x = (cue.pitch % 12) + (float) cue.gridOffset.x - 5.5f;
            y = cue.pitch / 12 + (float)cue.gridOffset.y - 3f;
        }

        AddTarget(x, y, (cue.tick - offset) / 480f , cue.tickLength / 120f, cue.velocity, cue.handType, cue.behavior);
    }

    public void AddTarget(float x, float y)
    {
        AddTarget(x, y, BeatTime(), 1, selectedVelocity, selectedHandType, selectedBehaviour);
    }

    public void AddTarget(float x, float y, float beatTime, float beatLength = 1, TargetVelocity velocity = TargetVelocity.Standard, TargetHandType handType = TargetHandType.Either, TargetBehavior behavior = TargetBehavior.Standard)
    {
        // Add to timeline
        var timelineClone = Instantiate(timelineNotePrefab, timelineNotes);
        timelineClone.transform.localPosition = new Vector3(beatTime, 0, 0);

        // Add to grid
        var gridClone = Instantiate(gridNotePrefab, gridNotes);
        gridClone.transform.localPosition = new Vector3(x, y, beatTime);

        gridClone.timelineTarget = timelineClone;
        timelineClone.timelineTarget = timelineClone;
        gridClone.gridTarget = gridClone;
        timelineClone.gridTarget = gridClone;

        gridClone.SetHandType(handType);
        gridClone.SetBehavior(behavior);
        gridClone.SetBeatLength(beatLength);
        gridClone.SetVelocity(velocity);
        
        notes.Add(gridClone);

        UpdateTrail();
    }
    
    public void DeleteTarget(Target target)
    {
        if (target == null) return;
        notes.Remove(target.gridTarget);
        var tl = target.timelineTarget.gameObject;
        var g = target.gridTarget.gameObject;
        Destroy(tl);
        Destroy(g);
    }

    public void Export()
    {
        Cue[] cues = new Cue[notes.Count];
        GridTarget[] orderedNotes = notes.OrderBy(v => v.transform.position.z).ToArray();
        for (int i = 0; i < notes.Count; i++)
        {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");
        
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "edica.cues"), json);

        json = JsonUtility.ToJson(songDesc, true);
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "song.desc"), json);

    }

    public void Import()
    {
        var descFiles = Directory.GetFiles(Environment.CurrentDirectory, "song.desc");
        if (descFiles.Length > 0)
        {
            string json = File.ReadAllText(descFiles[0]);
            songDesc = JsonUtility.FromJson<SongDesc>(json);
            SetOffset(songDesc.offset);
            SetSongID(songDesc.songID);
            SetSongTitle(songDesc.title);
            SetSongArtist(songDesc.artist);
            SetSongEndEvent(songDesc.songEndEvent.Replace("event:/song_end/song_end_", string.Empty));
            SetSongPreRoll(songDesc.prerollSeconds);
            SetSongAuthor(songDesc.author);

        } else
        {
            songDesc = new SongDesc();
        }

        var cueFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.cues");
        if (cueFiles.Length > 0) {
            string json = File.ReadAllText(cueFiles[0]);
            json = json.Replace("cues", "Items");
            Cue[] cues = JsonHelper.FromJson<Cue>(json);
            foreach (Cue cue in cues)
            {
                AddTarget(cue);
            }
        }

        var audioFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.ogg");
        if (audioFiles.Length > 0)
        {
            StartCoroutine(GetAudioClip(audioFiles[0]));
        }

    }

    IEnumerator GetAudioClip(string uri)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                aud.clip = myClip;
                previewAud.clip = myClip;

                SetBPM(songDesc.tempo);
                SetScale(20);
                Resources.FindObjectsOfTypeAll<OptionsMenu>().First().Init(bpm, offset, beatSnap, songid, songtitle, songartist, songendevent, songpreroll, songauthor);

                spectrogram.GetComponentInChildren<AudioWaveformVisualizer>().Init();
            }
        }
    }

    public void SetBehavior(TargetBehavior behavior)
    {
        selectedBehaviour = behavior;
    }

    public void SetVelocity(TargetVelocity velocity)
    {
        selectedVelocity = velocity;
    }

    public void SetPlaybackSpeed(float speed)
    {
        playbackSpeed = speed;
        aud.pitch = speed;
        previewAud.pitch = speed;
    }

    public void SetHandType(TargetHandType handType)
    {
        selectedHandType = handType;
    }

    public void SetBPM(float newBpm)
    {
        bpm = newBpm;
        songDesc.tempo = newBpm;
        SetScale(scale);
    }


    public void SetOffset(int newOffset)
    {
        offset = newOffset;
        songDesc.offset = newOffset;
    }

    public void SetSnap(int newSnap)
    {
        beatSnap = newSnap;
    }

    // .desc menu
    public void SetSongID(string newSongID)
    {
        songid = newSongID;
        songDesc.songID = newSongID;
    }

    public void SetSongTitle(string newSongTitle)
    {
        songtitle = newSongTitle;
        songDesc.title = newSongTitle;
    }

    public void SetSongArtist(string newSongArtist)
    {
        songartist = newSongArtist;
        songDesc.artist = newSongArtist;
    }

    public void SetSongEndEvent(string newSongEndEvent)
    {
        songendevent = newSongEndEvent;
        songDesc.songEndEvent = string.Concat("event:/song_end/song_end_", newSongEndEvent.ToUpper());
    }

    public void SetSongPreRoll(float newSongPreRoll)
    {
        songpreroll = newSongPreRoll;
        songDesc.prerollSeconds = newSongPreRoll;
    }

    public void SetSongAuthor(string newSongAuthor)
    {
        songauthor = newSongAuthor;
        songDesc.author = newSongAuthor;
    }

    public void SetBeatTime(float t)
    {
        timelineBG.material.SetTextureOffset("_MainTex", new Vector2 (((t * bpm/60 - offset/480f)/4f + scaleOffset), 1));

        timelineNotes.transform.localPosition = Vector3.left * (t * bpm / 60 - offset / 480f) / (scale / 20f);
        spectrogram.localPosition = Vector3.left * (t * bpm / 60) / (scale / 20f);

        gridNotes.transform.localPosition = Vector3.back * (t * bpm / 60 - offset / 480f);
    }

    public void SetScale(int newScale)
    {
        timelineBG.material.SetTextureScale("_MainTex", new Vector2(newScale/4f, 1));
        scaleOffset =  - newScale % 8 / 8f;

        spectrogram.localScale = new Vector3(aud.clip.length / 2 * bpm / 60 / (newScale/20f), 1, 1);

        timelineNotes.transform.localScale *= (float)scale/newScale;
        targetScale *= (float)newScale/scale;
        // fix scaling on all notes
        foreach (Transform note in timelineNotes.transform)
        {
            note.localScale = targetScale * Vector3.one;
        }
        scale = newScale;
    }

    private void UpdateTrail()
    {
        Vector3[] positions = new Vector3[gridNotes.childCount];
        for (int i = 0; i < gridNotes.transform.childCount; i++)
        {
            positions[i] = gridNotes.GetChild(i).localPosition;
        }
        positions = positions.OrderBy(v => v.z).ToArray();
        var liner = gridNotes.gameObject.GetComponentInChildren<LineRenderer>();
        liner.positionCount = gridNotes.childCount;
        liner.SetPositions(positions);
    }

    public void Update()
    {

        if (!paused)time += Time.deltaTime * playbackSpeed;

        if (hover)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.mouseScrollDelta.y > 0.1f)
                {
                    SetScale(scale - 1);
                }
                else if (Input.mouseScrollDelta.y < -0.1f)
                {
                    SetScale(scale + 1);
                }
            }
        }
        if(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && hover))
        if (Input.mouseScrollDelta.y > 0.1f)
        {
            time += BeatsToDuration(4f / beatSnap);
            time = SnapTime(time);
            aud.time = time;
            previewAud.time = time;
            if (paused) previewAud.Play();
        }
        else if (Input.mouseScrollDelta.y < -0.1f)
        {
            time -= BeatsToDuration(4f / beatSnap);
            time = SnapTime(time);
            if (time < 0) time = 0;
            aud.time = time;
            previewAud.time = time;
            if (paused) previewAud.Play();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (paused)
            {
                aud.Play();
                previewAud.Pause();
                paused = false;
            }
            else
            {
                aud.Pause();
                paused = true;
               // time = SnapTime(time);
                if (time < 0) time = 0;
                if (time > aud.clip.length) time = aud.clip.length;
            }
        }
        SetBeatTime(time);
        if (previewAud.time > time + previewDuration)
        {
            previewAud.Pause();
        }

        SetCurrentTime ();
        SetCurrentTick();
    }
    

    void OnMouseOver()
    {
        hover = true;
    }

    void OnMouseExit()
    {
        hover = false;
    }

    public float DurationToBeats(float t)
    {
        return t * bpm / 60;
    }

    public float BeatsToDuration(float beat)
    {
        return beat * 60 / bpm;
    }
    
    public float Snap(float beat)
    {
        return Mathf.Round(beat * beatSnap/4f) * 4f / beatSnap;
    }

    public float BeatTime()
    {
        return DurationToBeats(time) - offset / 480f;
    }

    public float SnapTime(float t)
    {
        return BeatsToDuration(Snap(DurationToBeats(t) - offset/480f)+offset/480f);
    }

    private void SetCurrentTime()
    {
        string minutes = Mathf.Floor((int)time / 60).ToString("00");
        string seconds = ((int)time % 60).ToString("00");

        curMinutes.text = minutes;
        curSeconds.text = seconds;
    }

    private void SetCurrentTick()
    {
        string currentTick = Mathf.Floor((int)BeatTime() * 480f).ToString();

        curTick.text = currentTick;
    }

}
