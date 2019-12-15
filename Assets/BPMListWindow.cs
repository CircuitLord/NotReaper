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

public class BPMListWindow : MonoBehaviour {
    public TMP_Text bpmTextList;

    void Start() {
        Vector3 defaultPos;
        defaultPos.x = 0;
        defaultPos.y = 0;
        defaultPos.z = -10.0f;
        gameObject.GetComponent<RectTransform>().localPosition = defaultPos;
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        gameObject.SetActive(false);
    }

    public void Activate(List<float> bpmList) {
        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        gameObject.SetActive(true);

        bpmTextList.text = "";
        foreach(float f in bpmList) {
            bpmTextList.text += f.ToString() + "\n";
        }
    }

    public void Deactivate() { 
        gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
        gameObject.SetActive(false);
    }
}
