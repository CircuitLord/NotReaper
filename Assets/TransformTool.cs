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
    [SerializeField] public Transform centerPoint;
    [SerializeField] ChainBuilderWindow chainBuilderWindow;
    public static TransformTool instance;
    RectTransform rectTransform;
    int lastSelectedTargetCount = 0;
    [SerializeField] SelectionMesh selectionMesh;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        instance = this;
    }

    private void Update()
    {
        if (timeline.selectedNotes.Count < 2 || chainBuilderWindow.gameObject.activeSelf)
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
        if (canvasGroup.alpha == 0f) return;
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.rotation = Quaternion.Euler(Vector3.zero);
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
        selectionMesh.GenerateMeshForTimeline();
    }

    public void SetPivot(Vector2 pivot)
    {
        
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }
    
    public void SetPivotToCenterPoint()
    {
        rectTransform.rotation = Quaternion.Euler(Vector3.zero);
        SetPivot(Vector2.up);
        Vector3 deltaPosition = rectTransform.position - centerPoint.position;
        deltaPosition *= ((Vector2.one + (Vector2.down * 2f)) / rectTransform.sizeDelta ) * (1 / canvas.transform.localScale.x);
        Vector2 pivot = new Vector2(deltaPosition.x * -1, deltaPosition.y + 1f);
        SetPivot(pivot);
    }

    public void RotateNotes(float angle)
    {
        timeline.Rotate(timeline.selectedNotes, angle, centerPoint.position);
    }
}
