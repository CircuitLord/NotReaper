using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public bool isWindowActive = false;
    public CanvasGroup canvasGroup;
    public float moveAmount;
    public bool animateChildren = true;
    public string windowName;
    public WindowManager windowManager;
    public bool canDrag = true;
    public bool draggedButton = false;
    public Vector2 dragStartPos;

    private void Start()
    {
        windowManager.windowList.Add(this);
        if (!isWindowActive)
        {
            gameObject.SetActive(false);
        }
    }
    public void ShowWindow()
    {
        transform.DOKill(true);
        transform.DOLocalMoveY(0f, 1f).From(-moveAmount).SetEase(Ease.OutQuart);
        canvasGroup.DOKill(true);
        canvasGroup.DOFade(1f, 1f).From(0f);
        gameObject.SetActive(true);
        isWindowActive = true;
        windowManager.windowCount++;
        if (animateChildren) { StartCoroutine(AnimateChildren()); };
    }
    public void HideWindow()
    {
        canvasGroup.DOFade(0f, 1f).OnComplete(() => { gameObject.SetActive(false); });
        isWindowActive = false;
        windowManager.windowCount--;
    }

    IEnumerator AnimateChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).DOKill(true);
            transform.GetChild(i).DOLocalMoveY(transform.GetChild(i).localPosition.y, 2f / (transform.childCount / (float)i)).From(transform.GetChild(i).localPosition.y - moveAmount).SetEase(Ease.OutQuart);
        }
        yield return null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggedButton)
        {
            transform.position = new Vector3(eventData.position.x + dragStartPos.x, eventData.position.y + dragStartPos.y, 0);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedButton = !canDrag ? true : false;
        dragStartPos = new Vector2(transform.position.x - eventData.position.x, transform.position.y - eventData.position.y);
    }
}