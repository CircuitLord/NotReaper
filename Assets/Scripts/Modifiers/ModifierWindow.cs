using NotReaper.Tools.ChainBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NotReaper.UI;

namespace NotReaper.Modifier
{
    public class ModifierWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ModifierHandler modifierCreator;

        public void OnPointerEnter(PointerEventData eventData)
        {
            modifierCreator.isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            modifierCreator.isHovering = false;
        }
    }
}

