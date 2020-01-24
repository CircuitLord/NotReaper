using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotReaper;
using TMPro;
using UnityEngine;
using NotReaper.UserInput;
using UnityEngine.EventSystems;
using NotReaper.Timing;

public class AddOrTrimAudioWindow : MonoBehaviour {
    public TMP_InputField timeLengthInput;
    public TMP_InputField beatLengthInput;
    
    [SerializeField] private Timeline timeline;

    void Start() {
        Vector3 defaultPos;
        defaultPos.x = 0;
        defaultPos.y = 0;
        defaultPos.z = -10.0f;
        gameObject.GetComponent<RectTransform>().localPosition = defaultPos;
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        gameObject.SetActive(true);


    }

    public void Deactivate() { 
        gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
        gameObject.SetActive(false);
    }

    Relative_QNT? GetTimeFromLabels() {
        float beatValue = 0;
        if(float.TryParse(beatLengthInput.text, out beatValue)) {
            if(beatValue > 0.0f) {
                return new Relative_QNT((long)Math.Round(Constants.PulsesPerQuarterNote * beatValue));
            }
        }

        float timeValue = 0.0f;
        if(float.TryParse(timeLengthInput.text, out timeValue)) {
            if(timeValue > 0.0f) {
                return Conversion.ToQNT(timeValue, timeline.tempoChanges[0].microsecondsPerQuarterNote);
            }
        }

        return null;
    }

    public void AddSilence() {
        var duration = GetTimeFromLabels();
        if(duration == null) {
            return;
        }

        if(duration.Value.tick < 0) {
            return;
        }

        timeline.RemoveOrAddTimeToAudio(duration.Value);
        Deactivate();
    }

    public void TrimAudio() {
        var duration = GetTimeFromLabels();
        if(duration == null) {
            return;
        }

        timeline.RemoveOrAddTimeToAudio(new Relative_QNT(-duration.Value.tick));
        Deactivate();
    }
}
