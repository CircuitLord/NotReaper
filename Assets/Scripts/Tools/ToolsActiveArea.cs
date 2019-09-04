using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NotReaper.Tools {

    public class ToolsActiveArea : MonoBehaviour {
        // Start is called before the first frame 

        //NM this is pointless
        
        public event Action<bool> ToolsUsableEvent;


        private void OnMouseEnter() {
            ToolsUsableEvent(true);

        }

        private void OnMouseExit() {
            //DisableHoverTarget();
            //EditorInput.isOverGrid = false;
        }
    }
}