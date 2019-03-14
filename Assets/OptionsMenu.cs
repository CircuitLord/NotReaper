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

    public TargetIcon hover;
    public NoteGrid noteGrid;

    public void Init(float bpm, int offset, int snap)
    {
        snapText.text = "beat snap 1/" + snap;
        SelectStandard();
        SelectLeftHand();
        bpmField.text = bpm.ToString();
        offsetField.text = offset.ToString();
        snapSlider.value = snap;
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
    }

    public void SelectStandard()
    {
        timeline.SetBehavior(TargetBehavior.Standard);
        hover.SetBehavior(TargetBehavior.Standard);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectHold()
    {
        timeline.SetBehavior(TargetBehavior.Hold);
        hover.SetBehavior(TargetBehavior.Hold);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectChain()
    {
        timeline.SetBehavior(TargetBehavior.Chain);
        hover.SetBehavior(TargetBehavior.Chain);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectChainStart()
    {
        timeline.SetBehavior(TargetBehavior.ChainStart);
        hover.SetBehavior(TargetBehavior.ChainStart);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectHorizontal()
    {
        timeline.SetBehavior(TargetBehavior.Horizontal);
        hover.SetBehavior(TargetBehavior.Horizontal);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectVertical()
    {
        timeline.SetBehavior(TargetBehavior.Vertical);
        hover.SetBehavior(TargetBehavior.Vertical);
        noteGrid.SetSnappingMode(NoteGrid.SnappingMode.Grid);
    }

    public void SelectMelee()
    {
        timeline.SetBehavior(TargetBehavior.Melee);
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
