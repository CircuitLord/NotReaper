using NotReaper.Tools.ChainBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChainBuilderWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ChainBuilder chainBuilder;

    public void OnPointerEnter(PointerEventData eventData)
    {
        chainBuilder.isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chainBuilder.isHovering = false;
    }
}
