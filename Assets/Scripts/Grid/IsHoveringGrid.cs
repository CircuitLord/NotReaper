using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Grid {


    public class IsHoveringGrid : MonoBehaviour {


        public HoverTarget hover;
        private void OnMouseOver() {
            EditorInput.isOverGrid = true;

            hover.TryEnable();

            
        }

        private void OnMouseExit() {
            EditorInput.isOverGrid = false;

            hover.TryDisable();
            
        }

    }

}