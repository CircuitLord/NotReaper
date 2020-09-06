
using Melanchall.DryWetMidi.Smf.Interaction;
using NotReaper;
using NotReaper.Models;
using NotReaper.Timing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimingPointItem : MonoBehaviour
{
    public TextMeshProUGUI timestamp;
    public TextMeshProUGUI bpm;
    public Button deleteButton;
    public TempoChange tempoData;
    [SerializeField] private Timeline timeline;


    public void SetInfoFromData(TempoChange tempoData)
    {
        this.tempoData = tempoData;
        bpm.text = $"{Constants.DisplayBPMFromMicrosecondsPerQuaterNote(tempoData.microsecondsPerQuarterNote)} bpm";
        timestamp.text = GetTimestampFromSeconds(tempoData.secondsFromStart);
    }

    public void RemoveTimingPoint()
    {
        timeline.SetBPM(tempoData.time, Constants.MicrosecondsPerQuarterNoteFromBPM(0f), true, tempoData.timeSignature);
        GameObject.FindObjectOfType<TimingPointsPanel>().UpdateTimingPointList(timeline.tempoChanges);
    }

    public void GoToTimingPoint()
    {
        StartCoroutine(timeline.AnimateSetTime(tempoData.time));
    }

    public string GetTimestampFromSeconds(float timeSeconds)
    {
        string minutes = Mathf.Floor((int)timeSeconds / 60).ToString("00");
        string seconds = ((int)timeSeconds % 60).ToString("00");
        return "<mspace=.5em>" + minutes + "</mspace>" + "<mspace=.4em>:</mspace>" + "<mspace=.5em>" + seconds + "</mspace>";
    }

}
