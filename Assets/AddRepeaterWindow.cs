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
using NotReaper.Models;

public class AddRepeaterWindow : MonoBehaviour {
    public TMP_InputField repeaterIdInput;
    public QNT_Timestamp? repeaterStart;
    public QNT_Timestamp? repeaterEnd;

    [SerializeField] private Timeline timeline;

    [SerializeField] GameObject startText;
    [SerializeField] GameObject endText;
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
        if(!timeline.paused) {
            timeline.TogglePlayback();
        }

        repeaterStart = null;
        repeaterEnd = null;

        startText.SetActive(false);
        endText.SetActive(false);

        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        gameObject.SetActive(true);

        repeaterIdInput.ActivateInputField();
    }

    public void Deactivate() { 
        gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
        repeaterIdInput.GetComponent<TMP_InputField>().ReleaseSelection();
        gameObject.SetActive(false);
    }

    public void SetBeginTime() {
        repeaterStart = Timeline.time;
        startText.SetActive(true);

        if (repeaterStart.HasValue && repeaterEnd.HasValue) {
            CreateRepeater();
        }
    }

    public void SetEndTime() {
        repeaterEnd = Timeline.time;
        endText.SetActive(true);

        if (repeaterStart.HasValue && repeaterEnd.HasValue) {
            CreateRepeater();
        }
    }

    public void CreateRepeater() {
        uint id = 0;
        if (uint.TryParse(repeaterIdInput.GetComponent<TMP_InputField>().text, out id)) {
            timeline.AddRepeaterSection(new RepeaterSection(id, repeaterStart.Value, repeaterEnd.Value));
            Deactivate();
        }
    }
}
