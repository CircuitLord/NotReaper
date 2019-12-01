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

public class DynamicBPMWindow : MonoBehaviour {
    public TMP_InputField dynamicBpmInput;

    [SerializeField] private Timeline timeline;

    void Start() {
        Vector3 defaultPos;
        defaultPos.x = 0;
        defaultPos.y = 0;
        defaultPos.z = -10.0f;
        gameObject.GetComponent<RectTransform>().localPosition = defaultPos;
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        gameObject.SetActive(false);
        dynamicBpmInput.GetComponent<TMP_InputField>().onSubmit.AddListener(delegate {AddDynamicBPM(); });
        //dynamicBpmInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate {Deactivate(); });
    }

    public void Activate() {
        if(!timeline.paused) {
            timeline.TogglePlayback();
        }

        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        gameObject.SetActive(true);

        double existingBPM = timeline.GetBpmFromTime(Timeline.time);
        if(existingBPM > 0) {
            dynamicBpmInput.GetComponent<TMP_InputField>().text = String.Format("{0:0.###}", existingBPM);
        }
        else {
           dynamicBpmInput.GetComponent<TMP_InputField>().text = "";
        }

        dynamicBpmInput.ActivateInputField();
    }

    public void Deactivate() { 
        gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
        dynamicBpmInput.GetComponent<TMP_InputField>().ReleaseSelection();
        gameObject.SetActive(false);
    }

    public void AddDynamicBPM() {
        double dynamicBpm = 0.0f;
        if(Double.TryParse(dynamicBpmInput.text, out dynamicBpm)) {
            timeline.SetBPM(Timeline.time, Constants.MicrosecondsPerQuarterNoteFromBPM(dynamicBpm));
            Deactivate();
        }
    }
}
