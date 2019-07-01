using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using SFB;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Timeline : MonoBehaviour {
    public bool SustainParticles = true;
    public OptionsMenu.DropdownToVelocity CurrentSound;
    public static Timeline TimelineStatic;
    public LoadModalInstance loadmodal;
    public DifficultySelection DifficultySelection_s;
    public float playbackSpeed = 1f;

    [SerializeField] private Renderer timelineBG;
    [SerializeField] private Transform timelineNotes;
    public static Transform gridNotesStatic;
    public static Transform timelineNotesStatic;
    [SerializeField] private Transform gridNotes;
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioSource previewAud;
    [SerializeField] private Transform spectrogram;
    [SerializeField] private TextMeshPro curMinutes;
    [SerializeField] private TextMeshPro curSeconds;
    [SerializeField] private TextMeshPro curTick;

    private Color leftColor;
    private Color rightColor;
    public Color bothColor;
    public Color neitherColor;

    List<GridTarget> notes;
    public static List<GridTarget> orderedNotes;
    List<TimelineTarget> notesTimeline;
    SongDescyay songDesc;

    public GridTarget gridNotePrefab;
    public TimelineTarget timelineNotePrefab;

    public float previewDuration = 0.1f;

    public static float time { get; private set; }

    private int beatSnap = 4;
    private int scale = 20;
    private float targetScale = 1f;
    private float scaleOffset = 0;
    private static float bpm = 60;
    private static int offset = 0;
    // .Desc menu
    private string songid = "";
    private string songtitle = "";
    private string songartist = "";
    private string songendevent = "";
    private float songpreroll = 0.0f;
    private string songauthor = "";
    private string mogg = "";

    private int seconds;
    public bool songLoaded = false;

    private TargetHandType selectedHandType = TargetHandType.Right;
    private TargetBehavior selectedBehaviour = TargetBehavior.Standard;
    private TargetVelocity selectedVelocity = TargetVelocity.Standard;

    private float timelineMaterialOffset;

    private bool hover = false;
    private bool paused = true;
    public bool projectStarted = false;

    private DirectorySecurity securityRules = new DirectorySecurity();
    private SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

    private void Start() {
        notes = new List<GridTarget>();
        orderedNotes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        securityRules.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, AccessControlType.Allow));

        gridNotesStatic = gridNotes;
        timelineNotesStatic = timelineNotes;
        TimelineStatic = this;

        //Modify the note colors
        leftColor = UserPrefsManager.leftColor;
        rightColor = UserPrefsManager.rightColor;
        bothColor = UserPrefsManager.bothColor;
        neitherColor = UserPrefsManager.neitherColor;

        loadmodal.LoadPanelStart();
    }

    void OnApplicationQuit() {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "\\temp\\");
        dir.Delete(true);
    }

    public void AddTarget(Cueyay cue) {
        float x = 0, y = 0;

        if (cue.behavior == TargetBehavior.Melee) {
            switch (cue.pitch) {
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
        } else {
            x = (cue.pitch % 12) + (float) cue.gridOffset.x - 5.5f;
            y = cue.pitch / 12 + (float) cue.gridOffset.y - 3f;
        }

        if (cue.tickLength / 480 >= 1)
            AddTarget(x, y, (cue.tick - offset) / 480f, cue.tickLength, cue.velocity, cue.handType, cue.behavior, false);
        else
            AddTarget(x, y, (cue.tick - offset) / 480f, 1, cue.velocity, cue.handType, cue.behavior, false);

    }

    public void AddTarget(float x, float y) {
        AddTarget(x, y, BeatTime(), 1, TargetVelocity.Standard, selectedHandType, selectedBehaviour, true);
    }

    public void AddTarget(float x, float y, float beatTime, float beatLength = 0.25f, TargetVelocity velocity = TargetVelocity.Standard, TargetHandType handType = TargetHandType.Either, TargetBehavior behavior = TargetBehavior.Standard, bool userAdded = false) {
        // Add to timeline
        var timelineClone = Instantiate(timelineNotePrefab, timelineNotes);
        timelineClone.transform.localPosition = new Vector3(beatTime, 0, 0);

        // Add to grid
        var gridClone = Instantiate(gridNotePrefab, gridNotes);
        gridClone.GetComponentInChildren<HoldController>().length.text = "" + beatLength;
        gridClone.transform.localPosition = new Vector3(x, y, beatTime);

        gridClone.timelineTarget = timelineClone;
        timelineClone.timelineTarget = timelineClone;
        gridClone.gridTarget = gridClone;
        timelineClone.gridTarget = gridClone;

        //set velocity
        if (userAdded) {
            switch (CurrentSound) {
                case OptionsMenu.DropdownToVelocity.Standard:
                    gridClone.velocity = TargetVelocity.Standard;
                    break;

                case OptionsMenu.DropdownToVelocity.Snare:
                    gridClone.velocity = TargetVelocity.Snare;
                    break;

                case OptionsMenu.DropdownToVelocity.Percussion:
                    gridClone.velocity = TargetVelocity.Percussion;
                    break;

                case OptionsMenu.DropdownToVelocity.ChainStart:
                    gridClone.velocity = TargetVelocity.ChainStart;
                    break;

                case OptionsMenu.DropdownToVelocity.Chain:
                    gridClone.velocity = TargetVelocity.Chain;
                    break;

                case OptionsMenu.DropdownToVelocity.Melee:
                    gridClone.velocity = TargetVelocity.Melee;
                    break;

                default:
                    gridClone.velocity = velocity;
                    break;

            }
        } else {
            gridClone.SetVelocity(velocity);
        }

        gridClone.SetHandType(handType);
        gridClone.SetBehavior(behavior);

        if (gridClone.behavior == TargetBehavior.Hold)
            gridClone.SetBeatLength(beatLength);
        else
            gridClone.SetBeatLength(0.25f);

        notes.Add(gridClone);
        notesTimeline.Add(timelineClone);

        orderedNotes = notes.OrderBy(v => v.transform.position.z).ToList();

        UpdateTrail();
        UpdateChainConnectors();
    }

    private void UpdateSustains() {
        foreach (var note in orderedNotes) {
            if (note.behavior == TargetBehavior.Hold) {
                if ((note.transform.position.z < 0) && (note.transform.position.z + note.beatLength > 0)) {
                    var particles = note.GetComponentInChildren<ParticleSystem>();
                    if (!particles.isEmitting) {
                        particles.Play();
                        var main = particles.main;
                        main.startColor = note.handType == TargetHandType.Left ? new Color(leftColor.r, leftColor.g, leftColor.b, 1) : new Color(rightColor.r, rightColor.g, rightColor.b, 1);
                    }

                    ParticleSystem.Particle[] parts = new ParticleSystem.Particle[particles.particleCount];
                    particles.GetParticles(parts);

                    for (int i = 0; i < particles.particleCount; ++i) {
                        parts[i].position = new Vector3(parts[i].position.x, parts[i].position.y, 0);
                    }

                    particles.SetParticles(parts, particles.particleCount);
                } else {
                    var particles = note.GetComponentInChildren<ParticleSystem>();
                    if (particles.isEmitting) {
                        particles.Stop();
                    }
                }
            }
        }
    }

    private void UpdateChords() {
        if (orderedNotes.Count > 0)
            foreach (var note in orderedNotes) {
                if (note && (Mathf.Round(note.transform.position.z) == 0 && note.behavior != TargetBehavior.ChainStart && note.behavior != TargetBehavior.Chain && note.behavior != TargetBehavior.HoldEnd)) {
                    LineRenderer lr = note.GetComponent<LineRenderer>();

                    List<Material> mats = new List<Material>();
                    lr.GetMaterials(mats);
                    Color activeColor = (note.handType == TargetHandType.Left) ? leftColor : rightColor;
                    mats[0].SetColor("_EmissionColor", new Color(activeColor.r, activeColor.g, activeColor.b, activeColor.a / 2));

                    var positionList = new List<Vector3>();
                    lr.SetPositions(positionList.ToArray());
                    lr.positionCount = 0;

                    foreach (var note2 in orderedNotes) {
                        if (note.transform.position.z == note2.transform.position.z && note != note2) {
                            var positionList2 = new List<Vector3>();

                            var offset = (note.handType == TargetHandType.Left) ? 0.01f : -0.01f;
                            positionList2.Add(note.transform.position + new Vector3(0, offset, 0));
                            positionList2.Add(note2.transform.position + new Vector3(0, offset, 0));
                            lr.positionCount = 2;

                            positionList2.Remove(new Vector3(0, 0, 1));

                            lr.SetPositions(positionList2.ToArray());

                        }

                    }

                } else {
                    LineRenderer lr = note.GetComponent<LineRenderer>();

                    if (lr.positionCount != 0) {
                        var positionList = new List<Vector3>();
                        lr.SetPositions(positionList.ToArray());
                        lr.positionCount = 0;
                    }
                }
            }

    }

    private void UpdateChainConnectors() {
        if (orderedNotes.Count > 0)
            foreach (var note in orderedNotes) {
                if (note.behavior == TargetBehavior.ChainStart) {
                    LineRenderer lr = note.GetComponentsInChildren<LineRenderer>() [1];

                    List<Material> mats = new List<Material>();
                    lr.GetMaterials(mats);
                    Color activeColor = note.handType == TargetHandType.Left ? leftColor : rightColor;
                    mats[0].SetColor("_Color", activeColor);
                    mats[0].SetColor("_EmissionColor", activeColor);

                    //clear pos list
                    lr.SetPositions(new Vector3[2]);
                    lr.positionCount = 2;

                    //set start pos
                    lr.SetPosition(0, note.transform.position);
                    lr.SetPosition(1, note.transform.position);

                    //add links in chain
                    List<GridTarget> chainLinks = new List<GridTarget>();
                    foreach (var note2 in orderedNotes) {
                        //if new start end old chain
                        if ((note.handType == note2.handType) && (note2.transform.position.z > note.transform.position.z) && (note2.behavior == TargetBehavior.ChainStart) && (note2 != note))
                            break;

                        if ((note.handType == note2.handType) && note2.transform.position.z > note.transform.position.z && note2.behavior == TargetBehavior.Chain) {
                            chainLinks.Add(note2);
                        }

                    }

                    note.chainedNotes = chainLinks;

                    //aply lines to chain links
                    if (chainLinks.Count > 0) {
                        var positionList = new List<Vector3>();
                        positionList.Add(new Vector3(note.transform.position.x, note.transform.position.y, 0));
                        foreach (var link in chainLinks) {
                            //add new
                            if (!positionList.Contains(link.transform.position)) {
                                positionList.Add(new Vector3(link.transform.position.x, link.transform.position.y, link.transform.position.z - note.transform.position.z));
                            }
                        }
                        lr.positionCount = positionList.Count;
                        positionList = positionList.OrderByDescending(v => v.z - note.transform.position.z).ToList();

                        for (int i = 0; i < positionList.Count; ++i) {
                            positionList[i] = new Vector3(positionList[i].x, positionList[i].y, 0);
                        }

                        var finalPositions = positionList.ToArray();
                        lr.SetPositions(finalPositions);
                    }
                }
            }

    }

    public void DeleteTarget(Target target) {
        if (target == null) return;
        notes.Remove(target.gridTarget);
        orderedNotes.Remove(target.gridTarget);
        var tl = target.timelineTarget.gameObject;
        var g = target.gridTarget.gameObject;
        Destroy(tl);
        Destroy(g);
        UpdateChainConnectors();
        UpdateChords();
    }

    private void DeleteTargets() {
        Debug.Log("del targs");
        foreach (GridTarget obj in notes) {
            if (obj)
                Destroy(obj.gameObject);
        }

        foreach (GridTarget obj in orderedNotes) {
            if (obj)
                Destroy(obj.gameObject);
        }

        foreach (TimelineTarget obj in notesTimeline) {
            if (obj)
                Destroy(obj.gameObject);
        }

        notes = new List<GridTarget>();
        orderedNotes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        var liner = gridNotes.gameObject.GetComponentInChildren<LineRenderer>();
        if (liner) {
            liner.SetPositions(new Vector3[0]);
            liner.positionCount = 0;
        }

        time = 0;
    }

    public void ChangeDifficulty() {
        Debug.Log("ChangeDifficulty");
        string dirpath = Application.persistentDataPath;

        string[] cuePath = Directory.GetFiles(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues");
        if (cuePath.Length > 0) {
            DeleteTargets();

            //load cues from temp
            string json = File.ReadAllText(cuePath[0]);
            json = json.Replace("cues", "Items");
            Cueyay[] cues = JsonHelper.FromJson<Cueyay>(json);
            foreach (Cueyay cue in cues) {
                AddTarget(cue);
            }
        } else {
            DeleteTargets();
        }

        foreach (GridTarget obj in notes) {
            Debug.Log(obj);
        }
    }

    public void Import() {
        DeleteTargets();

        //load cues from temp
        var cueFiles = StandaloneFileBrowser.OpenFilePanel("Import .cues", Application.persistentDataPath, "cues", false);
        if (cueFiles.Length > 0) {
            string json = File.ReadAllText(cueFiles[0]);
            json = json.Replace("cues", "Items");
            Cueyay[] cues = JsonHelper.FromJson<Cueyay>(json);
            foreach (Cueyay cue in cues) {
                AddTarget(cue);
            }
            songLoaded = true;
        } else {
            Debug.Log("cues not found");
        }

    }

    public void Save() {
        Cueyay[] cues = new Cueyay[notes.Count];
        for (int i = 0; i < notes.Count; i++) {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");

        string dirpath = Application.persistentDataPath;

        if (notes.Count > 0)
            File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);

        json = JsonUtility.ToJson(songDesc, true);
        File.WriteAllText(Path.Combine(dirpath + "\\temp\\", "song.desc"), json);
        FileInfo descFile = new FileInfo(Path.Combine(dirpath + "\\temp\\", "song.desc"));

        string[] oggPath = Directory.GetFiles(dirpath + "\\temp\\", "*.ogg");
        FileInfo oggFile = new FileInfo(oggPath[0]);

        List<FileInfo> files = new List<FileInfo>();
        files.Add(descFile);
        files.Add(oggFile);

        //push all .cues files to list
        var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
        if (cueFiles.Length > 0) {
            foreach (var cue in cueFiles) {
                FileInfo file = new FileInfo(cue);
                files.Add(file);
            }
        }

        string path = StandaloneFileBrowser.SaveFilePanel("Audica Save", Application.persistentDataPath, "Edica Save", "edica");
        //PlayerPrefs.SetString("previousSave", path);
        if (path.Length > 0)
            Compress(files, path);

    }

    public void Export() {
        string dirpath = Application.persistentDataPath;

        Cueyay[] cues = new Cueyay[notes.Count];
        for (int i = 0; i < notes.Count; i++) {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");

        if (notes.Count > 0)
            File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);

        string[] dirpatharr = StandaloneFileBrowser.OpenFolderPanel("Export Location", "", false);
        if (dirpatharr.Length > 0) {
            //push all .cues files to list
            List<FileInfo> files = new List<FileInfo>();

            var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
            if (cueFiles.Length > 0) {
                foreach (var cue in cueFiles) {
                    FileInfo file = new FileInfo(cue);
                    files.Add(file);
                }
            }

            //export desc    
            json = JsonUtility.ToJson(songDesc, true);
            try {
                File.WriteAllText(Path.Combine(dirpath + "\\temp\\", "song.desc"), json);
                FileInfo descfile = new FileInfo(dirpath + "\\temp\\song.desc");
                files.Add(descfile);
            } catch (IOException e) {
                Debug.Log(e.Data + "...DELETING");

                FileInfo fileToReplace = new FileInfo(dirpath + "\\temp\\song.desc");
                fileToReplace.Delete();
                try {
                    File.WriteAllText(Path.Combine(dirpath + "\\temp\\", "song.desc"), json);
                    FileInfo descfile = new FileInfo(dirpath + "\\temp\\song.desc");
                    files.Add(descfile);
                } catch (IOException e2) {
                    Debug.Log(e2.Message + "....No More attempts");
                }
            }

            //compress files and send to location
            Compress(files, dirpatharr[0] + "temp.zip");
            ZipFile.ExtractToDirectory(dirpatharr[0] + "temp.zip", dirpatharr[0]);

        }

    }

    public void ExportCueToTemp() {
        Cueyay[] cues = new Cueyay[notes.Count];
        for (int i = 0; i < notes.Count; i++) {
            cues[i] = orderedNotes[i].ToCue(offset);
        }

        string json = JsonHelper.ToJson(cues, true);
        json = json.Replace("Items", "cues");

        string dirpath = Application.persistentDataPath;

        if (notes.Count > 0)
            File.WriteAllText(Path.Combine(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues"), json);
    }

    public void Compress(List<FileInfo> files, string destination) {
        string dirpath = Application.persistentDataPath;

        if (!System.IO.Directory.Exists(dirpath + "\\TempSave\\")) {
            System.IO.Directory.CreateDirectory(dirpath + "\\TempSave\\", securityRules);

        }

        foreach (FileInfo fileToCompress in files) {
            fileToCompress.CopyTo(dirpath + "\\TempSave\\" + fileToCompress.Name, true);
        }

        try {
            ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
        } catch (IOException e) {
            Debug.Log(e.Message + "....Deleting File");
            FileInfo fileToReplace = new FileInfo(destination);
            fileToReplace.Delete();
            try {
                ZipFile.CreateFromDirectory(dirpath + "\\TempSave\\", destination);
            } catch (IOException e2) {
                Debug.Log(e2.Message + "....No More attempts");
            }

        }
        DirectoryInfo dir = new DirectoryInfo(dirpath + "\\TempSave\\");
        dir.Delete(true);
    }

    public void NewFile() {
        DeleteTargets();

        notes = new List<GridTarget>();
        notesTimeline = new List<TimelineTarget>();

        string dirpath = Application.persistentDataPath;

        //create the new song desc
        songDesc = new SongDescyay();

        //locate and copy ogg file to temp folder (used to save the project later)
        var audioFiles = StandaloneFileBrowser.OpenFilePanel("Import .ogg file", System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "ogg", false);
        if (audioFiles.Length > 0) {
            FileInfo oggFile = new FileInfo(audioFiles[0]);
            if (!System.IO.Directory.Exists(dirpath + "\\temp\\")) {
                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
            } else {
                DirectoryInfo direct = new DirectoryInfo(dirpath + "\\temp\\");
                direct.Delete(true);

                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
            }
            oggFile.CopyTo(dirpath + "\\temp\\" + oggFile.Name, true);

            //load ogg into audio clip
            if (audioFiles.Length > 0) {
                StartCoroutine(GetAudioClip(audioFiles[0]));
            } else {
                Debug.Log("ogg not found");
            }
        } else {
            Application.Quit();
        }

    }

    public void LoadFromFile(string edicaSave) {
        DeleteTargets();

        if (edicaSave.Length > 0) {
            PlayerPrefs.SetString("previousSave", edicaSave);

            //move zip to temp folder
            string dirpath = Application.persistentDataPath;
            if (!System.IO.Directory.Exists(dirpath + "\\temp\\")) {
                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
            } else {
                DirectoryInfo direct = new DirectoryInfo(dirpath + "\\temp\\");
                direct.Delete(true);

                System.IO.Directory.CreateDirectory(dirpath + "\\temp\\", securityRules);
            }

            //unzip save into temp
            ZipFile.ExtractToDirectory(edicaSave, dirpath + "\\temp\\");

            //load desc from temp
            var descFiles = Directory.GetFiles(dirpath + "\\temp\\", "song.desc");
            if (descFiles.Length > 0) {
                string json = File.ReadAllText(dirpath + "\\temp\\song.desc");
                songDesc = JsonUtility.FromJson<SongDescyay>(json);
                SetOffset(songDesc.offset);
                SetSongID(songDesc.songID);
                SetSongTitle(songDesc.title);
                SetSongArtist(songDesc.artist);
                SetSongEndEvent(songDesc.songEndEvent.Replace("event:/song_end/song_end_", string.Empty));
                SetSongPreRoll(songDesc.prerollSeconds);
                SetSongAuthor(songDesc.author);

            } else {
                Debug.Log("desc not found");
                songDesc = new SongDescyay();
            }

            //load cues from temp
            //var cueFile = Directory.GetFiles(dirpath + "\\temp\\", DifficultySelection_s.Value + ".cues");
            var cueFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.cues");
            if (cueFiles.Length > 0) {
                //figure out if it has difficulties (new edica file)
                bool isDifficultyNamed = false;
                string difficulty = "";
                foreach (var file in cueFiles) {
                    if (file.Contains("expert.cues")) {
                        isDifficultyNamed = true;
                        difficulty = "expert";
                    } else if (file.Contains("advanced.cues")) {
                        isDifficultyNamed = true;
                        difficulty = "advanced";
                    } else if (file.Contains("moderate.cues")) {
                        isDifficultyNamed = true;
                        difficulty = "moderate";
                    } else if (file.Contains("beginner.cues")) {
                        isDifficultyNamed = true;
                        difficulty = "beginner";
                    }
                }

                if (isDifficultyNamed) {
                    DifficultySelection_s.Value = difficulty;
                    DifficultySelection_s.selection.value = (int) DifficultySelection.Options.Parse(typeof(DifficultySelection.Options), difficulty);

                    //load .cues (delete first because changing difficulty creates targets)
                    DeleteTargets();
                    var cueFile = Directory.GetFiles(dirpath + "\\temp\\", difficulty + ".cues");
                    string json = File.ReadAllText(cueFile[0]);
                    json = json.Replace("cues", "Items");
                    Cueyay[] cues = JsonHelper.FromJson<Cueyay>(json);
                    foreach (Cueyay cue in cues) {
                        AddTarget(cue);
                    }
                } else //old edica format. Shove it into expert
                {
                    string json = File.ReadAllText(cueFiles[0]);
                    json = json.Replace("cues", "Items");
                    Cueyay[] cues = JsonHelper.FromJson<Cueyay>(json);
                    foreach (Cueyay cue in cues) {
                        AddTarget(cue);
                    }
                }

            } else {
                Debug.Log("cues not found");
                NewFile();
            }

            //load ogg into audio clip
            var audioFiles = Directory.GetFiles(dirpath + "\\temp\\", "*.ogg");
            if (audioFiles.Length > 0) {
                StartCoroutine(GetAudioClip(audioFiles[0]));
            } else {
                Debug.Log("ogg not found");
            }
        } else {
            NewFile();
        }
    }
    public void LoadPrevious() {
        string edicaSave = PlayerPrefs.GetString("previousSave");;
        LoadFromFile(edicaSave);
    }

    public void Load() {
        //open save
        string[] edicaSave = StandaloneFileBrowser.OpenFilePanel("Edica save file", Application.persistentDataPath, "edica", false);
        if (edicaSave.Length > 0)
            LoadFromFile(edicaSave[0]);
        else
            NewFile();
    }

    IEnumerator GetAudioClip(string uri) {
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log(www.error);
            } else {
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

    public void SetBehavior(TargetBehavior behavior) {
        selectedBehaviour = behavior;
    }

    public void SetVelocity(TargetVelocity velocity) {
        selectedVelocity = velocity;
    }

    public void SetPlaybackSpeed(float speed) {
        playbackSpeed = speed;
        aud.pitch = speed;
        previewAud.pitch = speed;
    }

    public void SetHandType(TargetHandType handType) {
        selectedHandType = handType;
    }

    public void SetBPM(float newBpm) {
        bpm = newBpm;
        songDesc.tempo = newBpm;
        SetScale(scale);
    }

    public void SetOffset(int newOffset) {
        offset = newOffset;
        songDesc.offset = newOffset;
    }

    public void SetSnap(int newSnap) {
        beatSnap = newSnap;
    }

    // .desc menu
    public void SetSongID(string newSongID) {
        songid = newSongID;
        songDesc.songID = newSongID;
    }

    public void SetSongTitle(string newSongTitle) {
        songtitle = newSongTitle;
        songDesc.title = newSongTitle;
    }

    public void SetSongArtist(string newSongArtist) {
        songartist = newSongArtist;
        songDesc.artist = newSongArtist;
    }

    public void SetSongEndEvent(string newSongEndEvent) {
        songendevent = newSongEndEvent;
        songDesc.songEndEvent = string.Concat("event:/song_end/song_end_", newSongEndEvent.ToUpper());
    }

    public void SetSongPreRoll(float newSongPreRoll) {
        songpreroll = newSongPreRoll;
        songDesc.prerollSeconds = newSongPreRoll;
    }

    public void SetSongAuthor(string newSongAuthor) {
        songauthor = newSongAuthor;
        songDesc.author = newSongAuthor;
    }

    public void SetMogg(string newmogg) {
        if (!mogg.Contains(".mogg")) {
            newmogg += ".mogg";
        }

        mogg = newmogg;
        songDesc.moggSong = mogg;

    }

    public void SetBeatTime(float t) {
        timelineBG.material.SetTextureOffset("_MainTex", new Vector2(((t * bpm / 60 - offset / 480f) / 4f + scaleOffset), 1));

        timelineNotes.transform.localPosition = Vector3.left * (t * bpm / 60 - offset / 480f) / (scale / 20f);
        spectrogram.localPosition = Vector3.left * (t * bpm / 60) / (scale / 20f);

        gridNotes.transform.localPosition = Vector3.back * (t * bpm / 60 - offset / 480f);
    }

    public void SetScale(int newScale) {
        timelineBG.material.SetTextureScale("_MainTex", new Vector2(newScale / 4f, 1));
        scaleOffset = -newScale % 8 / 8f;

        spectrogram.localScale = new Vector3(aud.clip.length / 2 * bpm / 60 / (newScale / 20f), 1, 1);

        timelineNotes.transform.localScale *= (float) scale / newScale;
        targetScale *= (float) newScale / scale;
        // fix scaling on all notes
        foreach (Transform note in timelineNotes.transform) {
            note.localScale = targetScale * Vector3.one;
        }
        scale = newScale;
    }

    public void UpdateTrail() {
        Vector3[] positions = new Vector3[gridNotes.childCount];
        for (int i = 0; i < gridNotes.transform.childCount; i++) {
            positions[i] = gridNotes.GetChild(i).localPosition;
        }
        positions = positions.OrderBy(v => v.z).ToArray();
        var liner = gridNotes.gameObject.GetComponentInChildren<LineRenderer>();
        liner.positionCount = gridNotes.childCount;
        liner.SetPositions(positions);
    }

    public void Update() {
        if (orderedNotes.Count > 0)
            foreach (var note in orderedNotes) {

                if (note.behavior == TargetBehavior.ChainStart && note) {
                    if (note.chainedNotes.Count > 0) {
                        if ((note.transform.position.z < 0 && 0 > note.chainedNotes.Last().transform.position.z) || (note.transform.position.z > 0 && 0 < note.chainedNotes.Last().transform.position.z)) {
                            note.GetComponentsInChildren<LineRenderer>() [1].enabled = false;
                        } else {
                            note.GetComponentsInChildren<LineRenderer>() [1].enabled = true;
                        }
                    }
                }

                if (note.behavior == TargetBehavior.Hold && note) {
                    var length = Mathf.Ceil(float.Parse(note.GetComponentInChildren<HoldController>().length.text)) / 480;
                    if (note.gridTarget.beatLength != length) {
                        note.gridTarget.SetBeatLength(length);
                        Debug.Log(note.beatLength);
                    }
                }
            }

        UpdateChords();

        if (SustainParticles)
            UpdateSustains();

        if (!paused) time += Time.deltaTime * playbackSpeed;

        if (hover) {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                if (Input.mouseScrollDelta.y > 0.1f) {
                    SetScale(scale - 1);
                } else if (Input.mouseScrollDelta.y < -0.1f) {
                    SetScale(scale + 1);
                }
            }
        }
        if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && hover))

            //Fixed the timeline scroll direction.

            if (Input.mouseScrollDelta.y < -0.1f) {
                time += BeatsToDuration(4f / beatSnap);
                time = SnapTime(time);
                aud.time = time;
                previewAud.time = time;
                if (paused) previewAud.Play();
            }
        else if (Input.mouseScrollDelta.y > 0.1f) {
            time -= BeatsToDuration(4f / beatSnap);
            time = SnapTime(time);
            if (time < 0) time = 0;
            aud.time = time;
            previewAud.time = time;
            if (paused) previewAud.Play();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (paused) {
                aud.Play();
                previewAud.Pause();
                paused = false;
            } else {
                aud.Pause();
                paused = true;

                //Uncommented
                time = SnapTime(time);
                if (time < 0) time = 0;
                if (time > aud.clip.length) time = aud.clip.length;

            }
        }
        SetBeatTime(time);
        if (previewAud.time > time + previewDuration) {
            previewAud.Pause();
        }

        previewAud.volume = aud.volume;

        SetCurrentTime();
        SetCurrentTick();
    }

    void OnMouseOver() {
        hover = true;
    }

    void OnMouseExit() {
        hover = false;
    }

    public float GetPercentagePlayed() {
        if (aud.clip != null)
            return (time / aud.clip.length);

        else
            return 0;
    }

    public static float DurationToBeats(float t) {
        return t * bpm / 60;
    }

    public float BeatsToDuration(float beat) {
        return beat * 60 / bpm;
    }

    public float Snap(float beat) {
        return Mathf.Round(beat * beatSnap / 4f) * 4f / beatSnap;
    }

    public static float BeatTime() {
        return DurationToBeats(time) - offset / 480f;
    }

    public float SnapTime(float t) {
        return BeatsToDuration(Snap(DurationToBeats(t) - offset / 480f) + offset / 480f);
    }

    private void SetCurrentTime() {
        string minutes = Mathf.Floor((int) time / 60).ToString("00");
        string seconds = ((int) time % 60).ToString("00");

        curMinutes.text = minutes;
        curSeconds.text = seconds;
    }

    private void SetCurrentTick() {
        string currentTick = Mathf.Floor((int) BeatTime() * 480f).ToString();

        curTick.text = currentTick;
    }

    public void SustainParticlesToggle(Toggle tog) {
        SustainParticles = tog.isOn;

        if (!SustainParticles) {
            foreach (var note in orderedNotes) {
                if (note.behavior == TargetBehavior.Hold) {
                    var particles = note.GetComponentInChildren<ParticleSystem>();
                    if (particles.isEmitting) {
                        particles.Stop();
                    }
                }
            }
        }
    }
}