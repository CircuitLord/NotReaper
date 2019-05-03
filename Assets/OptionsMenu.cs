using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Timeline timeline;

    public InputField bpmField;
    public InputField offsetField;
    public Slider snapSlider;
    public Text snapText;
    public Text playSpeedText;

    public Dropdown SoundDropdown;

    // .Desc inputs
    public InputField songID;
    public InputField songTitle;
    public InputField songArtist;
    public InputField songEndEvent;
    public InputField songPreRoll;
    public InputField songAuthor;
    public InputField Moggsong;


    public TargetIcon hover;
    public NoteGrid noteGrid;

    public void Init(float bpm, int offset, int snap, string songid, string songtitle, string songartist, string songendevent, float songpreroll, string songauthor)
    {
        snapText.text = "beat snap 1/" + snap;
        SelectStandard();
        SelectLeftHand();
        bpmField.text = bpm.ToString();
        offsetField.text = offset.ToString();
        snapSlider.value = snap;
        // .desc menu
        songID.text = songid;
        songTitle.text = songtitle;
        songArtist.text = songartist;
        songEndEvent.text = songendevent;
        songPreRoll.text = songpreroll.ToString();
        songAuthor.text = songauthor;
    }

    public enum DropdownToVelocity
    {
        Standard = 0, Snare = 1, Percussion = 2,ChainStart = 3, Chain = 4, Melee = 5
    }
    public void SoundWasChanged(Dropdown dpd)
    {
        timeline.CurrentSound = (DropdownToVelocity)dpd.value;
    }


    public void BPMWasChanged()
    {
        float newBpm = float.Parse(bpmField.text);
        timeline.SetBPM(newBpm);
    }
    
    public void OffsetWasChanged()
    {
        int newOffset = int.Parse(offsetField.text);
        timeline.SetOffset(newOffset);
    }

    public void SnapWasChanged(float rawSnap)
    {
        int snap = Mathf.RoundToInt(rawSnap);
        timeline.SetSnap(snap);
        snapText.text = "beat snap 1/" + snap;
    }

    public void PlaybackWasChanged(float speed)
    {
        timeline.SetPlaybackSpeed(speed);
        playSpeedText.text = "Speed " + Mathf.FloorToInt(speed * 100) + "%";
    }
    // .Desc menu
    public void SongIDWasChanged()
    {
        string newSongID = songID.text;
        timeline.SetSongID(newSongID);
    }

    public void SongTitleWasChanged()
    {
        string newSongTitle = songTitle.text;
        timeline.SetSongTitle(newSongTitle);
    }

    public void SongArtistWasChanged()
    {
        string newSongArtist = songArtist.text;
        timeline.SetSongArtist(newSongArtist);
    }

    public void SongEndEventWasChanged()
    {
        string newSongEndEvent = songEndEvent.text;
        timeline.SetSongEndEvent(newSongEndEvent);
    }

    public void SongPreRollWasChanged()
    {
        float newSongPreRoll = float.Parse(songPreRoll.text);
        timeline.SetSongPreRoll(newSongPreRoll);
    }

    public void SongAuthorWasChanged()
    {
        string newSongAuthor = songAuthor.text;
        timeline.SetSongAuthor(newSongAuthor);
    }

    public void MoggWasChanged()
    {
        string newMog = Moggsong.text;
        timeline.SetMogg(newMog);
    }

    public void SelectStandard()
    {
        timeline.SetBehavior(TargetBehavior.Standard);
        timeline.SetVelocity(TargetVelocity.Standard);
        SoundDropdown.value = (int)DropdownToVelocity.Standard;
        hover.SetBehavior(TargetBehavior.Standard);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectHold()
    {
        timeline.SetBehavior(TargetBehavior.Hold);
        timeline.SetVelocity(TargetVelocity.Hold);
        SoundDropdown.value = (int)DropdownToVelocity.Standard;
        hover.SetBehavior(TargetBehavior.Hold);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectChain()
    {
        timeline.SetBehavior(TargetBehavior.Chain);
        timeline.SetVelocity(TargetVelocity.Chain);
        SoundDropdown.value = (int)DropdownToVelocity.Chain;
        hover.SetBehavior(TargetBehavior.Chain);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectChainStart()
    {
        timeline.SetBehavior(TargetBehavior.ChainStart);
        timeline.SetVelocity(TargetVelocity.ChainStart);
        SoundDropdown.value = (int)DropdownToVelocity.ChainStart;
        hover.SetBehavior(TargetBehavior.ChainStart);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectHorizontal()
    {
        timeline.SetBehavior(TargetBehavior.Horizontal);
        timeline.SetVelocity(TargetVelocity.Horizontal);
        SoundDropdown.value = (int)DropdownToVelocity.Standard;
        hover.SetBehavior(TargetBehavior.Horizontal);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectVertical()
    {
        timeline.SetBehavior(TargetBehavior.Vertical);
        timeline.SetVelocity(TargetVelocity.Vertical);
        SoundDropdown.value = (int)DropdownToVelocity.Standard;
        hover.SetBehavior(TargetBehavior.Vertical);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectMelee()
    {
        timeline.SetBehavior(TargetBehavior.Melee);
        timeline.SetVelocity(TargetVelocity.Melee);
        SoundDropdown.value = (int)DropdownToVelocity.Melee;
        hover.SetBehavior(TargetBehavior.Melee);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Melee);
    }

    public void SelectLeftHand()
    {
        timeline.SetHandType(TargetHandType.Left);
        hover.SetHandType(TargetHandType.Left);
    }

    public void SelectRightHand()
    {
        timeline.SetHandType(TargetHandType.Right);
        hover.SetHandType(TargetHandType.Right);
    }

    public void SelectEitherHand()
    {
        timeline.SetHandType(TargetHandType.Either);
        hover.SetHandType(TargetHandType.Either);
    }

    public void SelectNoHand()
    {
        timeline.SetHandType(TargetHandType.None);
        hover.SetHandType(TargetHandType.None);
    }
}
