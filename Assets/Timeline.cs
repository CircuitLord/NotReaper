using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Security.AccessControl;
using System.Security.Principal;
using SFB;

public class Timeline : MonoBehaviour
{    
    public LoadModalInstance loadmodal;
    public DifficultySelection DifficultySelection_s;
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
    List<TimelineTarget> notesTimeline;
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
    public bool projectStarted = false;

    private DirectorySecurity securityRules = new DirectorySecurity();
    private SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);


    private void Start()
    {
        notes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        securityRules.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, AccessControlType.Allow));


        loadmodal.LoadPanelStart();
    }

    void OnApplicationQuit()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "\\temp\\");
        dir.Delete(true);
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
            x = (cue.pitch % 12) + (float)cue.gridOffset.x - 5.5f;
            y = cue.pitch / 12 + (float)cue.gridOffset.y - 3f;
        }

        AddTarget(x, y, (cue.tick - offset) / 480f, cue.tickLength / 120f, cue.velocity, cue.handType, cue.behavior);
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
        notesTimeline.Add(timelineClone);

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

    private void DeleteTargets()
    {
        foreach (GridTarget obj in notes)
        {
            if(obj)
                Destroy(obj.gameObject);
        }

        foreach (TimelineTarget obj in notesTimeline)
        {
            if(obj)
                Destroy(obj.gameObject);
        }

        notes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        var liner = gridNotes.gameObject.GetComponentInChildren<LineRenderer>();
        liner.SetPositions(new Vector3[0]);
        liner.positionCount = 0;

        time = 0;
    }

    public void ChangeDifficulty()
    {
        string dirpath = Application.persistentDataPath;

        string[] cuePath = Directory.GetFiles(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues");
        if (cuePath.Length > 0)
        {
            DeleteTargets();

            //load cues from temp
            string json = File.ReadAllText(cuePath[0]);
            json = json.Replace("cues", "Items");
            Cue[] cues = JsonHelper.FromJson<Cue>(json);
            foreach (Cue cue in cues)
            {
                AddTarget(cue);
            }
        }
        else
        {
            DeleteTargets();
        }
    }

    public void Import()
    {
        DeleteTargets();

        //load cues from temp
            var cueFiles = StandaloneFileBrowser.OpenFilePanel("Import .cues", Application.persistentDataPath, "cues", false);
            if (cueFiles.Length > 0)
            {
                string json = File.ReadAllText(cueFiles[0]);
                json = json.Replace("cues", "Items");
                Cue[] cues = JsonHelper.FromJson<Cue>(json);
                foreach (Cue cue in cues)
                {
                    AddTarget(cue);
                }
            }
            else
            {
                Debug.Log("cues not found");
            }

    }

    public void Save()
    {      
        Cue[] cues = new Cue[notes.Count];
        GridTarget[] orderedNotes = notes.OrderBy(v => v.transform.position.z).ToArray();
        for (int i = 0; i < notes.Count; i++)
        {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");

        string dirpath = Application.persistentDataPath;

        if(notes.Count>0)
            File.WriteAllText(Path.Combine(dirpath  + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);


        json = JsonUtility.ToJson(songDesc, true);
        File.WriteAllText(Path.Combine(dirpath  + "\\temp\\", "song.desc"), json);
        FileInfo descFile = new FileInfo(Path.Combine(dirpath  + "\\temp\\", "song.desc"));

        string[] oggPath = Directory.GetFiles(dirpath + "\\temp\\", "*.ogg");
        FileInfo oggFile = new FileInfo(oggPath[0]);


        List<FileInfo> files = new List<FileInfo>();
        files.Add(descFile);
        files.Add(oggFile);

        //push all .cues files to list
        var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
        if (cueFiles.Length > 0)
        {
            foreach(var cue in cueFiles)
            {
                FileInfo file = new FileInfo(cue);
                files.Add(file);
            }
        }

        string path = StandaloneFileBrowser.SaveFilePanel("Audica Save", Application.persistentDataPath, "Edica Save", "edica");
        //PlayerPrefs.SetString("previousSave", path);
        if(path.Length > 0)
            Compress(files, path);

    }

    public void Export()
    {

        string[] dirpatharr = StandaloneFileBrowser.OpenFolderPanel("Export Location", "", false);
        if (dirpatharr.Length > 0)
        {
            //push all .cues files to list
            List<FileInfo> files = new List<FileInfo>();
            string dirpath = Application.persistentDataPath;


            var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
            if (cueFiles.Length > 0)
            {
                foreach (var cue in cueFiles)
                {
                    FileInfo file = new FileInfo(cue);
                    files.Add(file);
                }
            }


            //export desc    
            string json = JsonUtility.ToJson(songDesc, true);
            File.WriteAllText(Path.Combine(dirpath + "\\temp\\", "song.desc"), json);
            FileInfo descfile = new FileInfo(dirpath + "\\temp\\song.desc");
            files.Add(descfile);

            //compress files and send to location
            Compress(files,dirpatharr[0]+"temp.zip");
            ZipFile.ExtractToDirectory(dirpatharr[0] + "temp.zip", dirpatharr[0]);

        }

    }

    public void ExportCueToTemp()
    {
        Cue[] cues = new Cue[notes.Count];
        GridTarget[] orderedNotes = notes.OrderBy(v => v.transform.position.z).ToArray();
        for (int i = 0; i < notes.Count; i++)
        {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");

        string dirpath = Application.persistentDataPath;

        if(notes.Count>0)
            File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);
    }


    public void Compress(List<FileInfo> files, string destination)
    {
        string dirpath = Application.persistentDataPath;

        if (!System.IO.Directory.Exists(dirpath + "\\TempSave\\"))
        {
            System.IO.Directory.CreateDirectory(dirpath + "\\TempSave\\", securityRules);

        }

        foreach (FileInfo fileToCompress in files)
        {
            fileToCompress.CopyTo(dirpath + "\\TempSave\\" + fileToCompress.Name, true);
        }

        try
        {
            ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message + "....Deleting File");
            FileInfo fileToReplace = new FileInfo(destination);
            fileToReplace.Delete();
            try
            {
                ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
            }
            catch (IOException e2)
            {
                Debug.Log(e2.Message + "....No More attempts");
            }

        }
        DirectoryInfo dir = new DirectoryInfo(dirpath + "\\TempSave\\");
        dir.Delete(true);
    }

    public void NewFile()
    {
        DeleteTargets();

        notes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        string dirpath = Application.persistentDataPath;

        //create the new song desc
        songDesc = new SongDesc();

        //locate and copy ogg file to temp folder (used to save the project later)
        var audioFiles = StandaloneFileBrowser.OpenFilePanel("Import .ogg file", System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "ogg",false);
        if (audioFiles.Length > 0)
        {
            FileInfo oggFile = new FileInfo(audioFiles[0]);
            if (!System.IO.Directory.Exists(dirpath + "\\temp\\"))
            {
                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\",securityRules);
            }
            else
            {
                DirectoryInfo direct = new DirectoryInfo(dirpath + "\\temp\\");
                direct.Delete(true);

                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\",securityRules);
            }
            oggFile.CopyTo(dirpath + "\\temp\\" + oggFile.Name, true);

            //load ogg into audio clip
            if (audioFiles.Length > 0)
            {
                StartCoroutine(GetAudioClip(audioFiles[0]));
            }
            else
            {
                Debug.Log("ogg not found");
            }
        }
        else
        {
            Application.Quit();
        }

    }

    public void LoadFromFile(string edicaSave)
    {
        DeleteTargets();        
        
        if (edicaSave.Length > 0)
        {
            PlayerPrefs.SetString("previousSave",edicaSave);

            //move zip to temp folder
            string dirpath = Application.persistentDataPath;
            if (!System.IO.Directory.Exists(dirpath + "\\temp\\"))
            {
                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\",securityRules);
            }
            else
            {
                DirectoryInfo direct = new DirectoryInfo(dirpath + "\\temp\\");
                direct.Delete(true);

                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\",securityRules);
            }

            //unzip save into temp
            ZipFile.ExtractToDirectory(edicaSave, dirpath + "\\temp\\");


            //load desc from temp
            var descFiles = Directory.GetFiles(dirpath + "\\temp\\", "song.desc");
            if (descFiles.Length > 0)
            {
                string json = File.ReadAllText(dirpath + "\\temp\\song.desc");
                songDesc = JsonUtility.FromJson<SongDesc>(json);
                SetOffset(songDesc.offset);
                SetSongID(songDesc.songID);
                SetSongTitle(songDesc.title);
                SetSongArtist(songDesc.artist);
                SetSongEndEvent(songDesc.songEndEvent.Replace("event:/song_end/song_end_", string.Empty));
                SetSongPreRoll(songDesc.prerollSeconds);
                SetSongAuthor(songDesc.author);

            }
            else
            {
                Debug.Log("desc not found");
                songDesc = new SongDesc();
            }

            //load cues from temp
            //var cueFile = Directory.GetFiles(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues");
            var cueFiles = Directory.GetFiles(dirpath + "\\temp\\","*.cues");
            if (cueFiles.Length > 0)
            {
                //figure out if it has difficulties (new edica file)
                bool isDifficultyNamed = false;
                string difficulty = "";
                foreach(var file in cueFiles)
                {
                    if(file.Contains("expert.cues"))
                    {
                        isDifficultyNamed = true;
                        difficulty = "expert";
                    }
                    else if(file.Contains("advanced.cues"))
                    {
                        isDifficultyNamed = true;
                        difficulty = "advanced";                    
                    }
                    else if(file.Contains("moderate.cues"))
                    {
                        isDifficultyNamed = true;
                        difficulty = "moderate";                  
                    }
                    else if(file.Contains("beginner.cues"))
                    {
                        isDifficultyNamed = true;
                        difficulty = "beginner";
                    }
                }

                if (isDifficultyNamed)
                {
                    DifficultySelection_s.Value = difficulty;
                    DifficultySelection_s.selection.value = (int)DifficultySelection.Options.Parse(typeof(DifficultySelection.Options), difficulty);

                    //load .cues
                    var cueFile = Directory.GetFiles(dirpath + "\\temp\\", difficulty + ".cues");
                    string json = File.ReadAllText(cueFile[0]);
                    json = json.Replace("cues", "Items");
                    Cue[] cues = JsonHelper.FromJson<Cue>(json);
                    foreach (Cue cue in cues)
                    {
                        AddTarget(cue);
                    }
                }

                else //old edica format. Shove it into expert
                {
                    string json = File.ReadAllText(cueFiles[0]);
                    json = json.Replace("cues", "Items");
                    Cue[] cues = JsonHelper.FromJson<Cue>(json);
                    foreach (Cue cue in cues)
                    {
                        AddTarget(cue);
                    }
                }
                
                
            }
            else
            {
                Debug.Log("cues not found");
                NewFile();
            }


            //load ogg into audio clip
            var audioFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.ogg");
            if (audioFiles.Length > 0)
            {
                StartCoroutine(GetAudioClip(audioFiles[0]));
            }
            else
            {
                Debug.Log("ogg not found");
            }
        }
        else
        {
            NewFile();
        }
    }
    public void LoadPrevious()
    {
        string edicaSave = PlayerPrefs.GetString("previousSave");;   
        LoadFromFile(edicaSave);
    }
    
    public void Load()
    {
        //open save
        string[] edicaSave = StandaloneFileBrowser.OpenFilePanel("Edica save file", Application.persistentDataPath, "edica",false);
        if(edicaSave.Length > 0)
            LoadFromFile(edicaSave[0]);
        else
            NewFile();
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
        timelineBG.material.SetTextureOffset("_MainTex", new Vector2(((t * bpm / 60 - offset / 480f) / 4f + scaleOffset), 1));

        timelineNotes.transform.localPosition = Vector3.left * (t * bpm / 60 - offset / 480f) / (scale / 20f);
        spectrogram.localPosition = Vector3.left * (t * bpm / 60) / (scale / 20f);

        gridNotes.transform.localPosition = Vector3.back * (t * bpm / 60 - offset / 480f);
    }

    public void SetScale(int newScale)
    {
        timelineBG.material.SetTextureScale("_MainTex", new Vector2(newScale / 4f, 1));
        scaleOffset = -newScale % 8 / 8f;

        spectrogram.localScale = new Vector3(aud.clip.length / 2 * bpm / 60 / (newScale / 20f), 1, 1);

        timelineNotes.transform.localScale *= (float)scale / newScale;
        targetScale *= (float)newScale / scale;
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

        if (!paused) time += Time.deltaTime * playbackSpeed;

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
        if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && hover))
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

        SetCurrentTime();
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
        return Mathf.Round(beat * beatSnap / 4f) * 4f / beatSnap;
    }

    public float BeatTime()
    {
        return DurationToBeats(time) - offset / 480f;
    }

    public float SnapTime(float t)
    {
        return BeatsToDuration(Snap(DurationToBeats(t) - offset / 480f) + offset / 480f);
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
