using System.Collections;
using System.Collections.Generic;
using NotReaper;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;
using UnityEngine.UI;
using NotReaper.Timing;
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
    public NoteGridSnap noteGrid;


    public void Init(float bpm, int offset, int snap, string songid, string songtitle, string songartist, string songendevent, float songpreroll, string songauthor) {
        snapText.text = "beat snap 1/" + snap;
        //SelectStandard();
        //SelectLeftHand();
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




    public void BPMWasChanged() {
        float newBpm = float.Parse(bpmField.text);
        timeline.SetBPM(new QNT_Timestamp(0), Constants.MicrosecondsPerQuarterNoteFromBPM(newBpm), false);
    }

    public void OffsetWasChanged() {
        int newOffset = int.Parse(offsetField.text);
        timeline.SetOffset(new Relative_QNT(newOffset));
    }

    public void SnapWasChanged(float rawSnap) {
        int snap = Mathf.RoundToInt(rawSnap);
        timeline.SetSnap(snap);
        snapText.text = "beat snap 1/" + snap;
    }

    public void PlaybackWasChanged(float speed) {
        timeline.SetPlaybackSpeed(speed);
        playSpeedText.text = "Speed " + Mathf.FloorToInt(speed * 100) + "%";
    }


   


}