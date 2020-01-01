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

public class CountInWindow : MonoBehaviour {
    public TMP_InputField lengthInput;
    
    [SerializeField] private Timeline timeline;

    void Start() {
        Vector3 defaultPos;
        defaultPos.x = 0;
        defaultPos.y = 0;
        defaultPos.z = -10.0f;
        lengthInput.text = "8";
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

    public void PreviewCountIn() {
        uint beats = 0;
        if(uint.TryParse(lengthInput.text, out beats)) {
            timeline.PreviewCountIn(beats);
        }
    }

    public void GenerateCountIn() {
        uint beats = 0;
        if(uint.TryParse(lengthInput.text, out beats)) {
            timeline.GenerateCountIn(beats);
        }
    }
}
