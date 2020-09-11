using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class OnHoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string onHoverTipText;
    [SerializeField] bool animate;
    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTips.I.SetText(onHoverTipText);
        if (animate)
        {
            transform.DOKill(true);
            transform.DOPunchScale(new Vector3(0.04f, 0.1f, 0.04f), 0.13f, 2, 0.2f); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTips.I.SetText("");
    }
}
