using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Grid {


    public class IsHoveringGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public HoverTarget hover;


        public void OnPointerEnter(PointerEventData eventData)
        {
            EditorInput.isOverGrid = true;
            hover.TryEnable();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EditorInput.isOverGrid = false;
            hover.TryDisable();
        }
    }

}