using NotReaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TransformTool : MonoBehaviour
{
    [SerializeField] Timeline timeline;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI countLabel;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform centerPoint;
    RectTransform rectTransform;
    int lastSelectedTargetCount = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (timeline.selectedNotes.Count < 1)
        {
            canvasGroup.alpha = 0f;
            return;
        }
        else if(timeline.selectedNotes.Count != lastSelectedTargetCount)
        {
            canvasGroup.alpha = 1f;
            lastSelectedTargetCount = timeline.selectedNotes.Count;
            UpdateOverlay();
        }
    }

    [ContextMenu("Debug centering")]
    public void UpdateOverlay()
    {
        float minX = 999f, minY = 999f, maxX = -999f, maxY = -999f;
        float totalX = 0f, totalY = 0f;
        foreach (var selectedTarget in timeline.selectedNotes)
        {
            NotReaper.Targets.TargetData current = selectedTarget.data;
            if (current.x > maxX) maxX = current.x;
            if (current.y > maxY) maxY = current.y;
            if (current.x < minX) minX = current.x;
            if (current.y < minY) minY = current.y;
            totalX += current.x;
            totalY += current.y;
        }
        float centerX = totalX / timeline.selectedNotes.Count;
        float centerY = totalY / timeline.selectedNotes.Count;

        countLabel.text = lastSelectedTargetCount.ToString();
        transform.position = new Vector3(minX, maxY);
        rectTransform.sizeDelta = new Vector2(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY)) * (1 / canvas.transform.localScale.x);
        centerPoint.localPosition = new Vector2((float)(rectTransform.sizeDelta.x * 0.5), (float)-(rectTransform.sizeDelta.y * 0.5));
    }
}
