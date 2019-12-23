using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Grid {


    public class IsHoveringGrid : MonoBehaviour {

        private Bounds gridRect;
        public HoverTarget hover;

        private void Start() {
            var transforms = GetComponentsInChildren<Renderer>(false);
            foreach(Renderer renderer in transforms) {
                gridRect.Encapsulate(renderer.bounds);
            }
        }

        private void Update() {
            Vector3 cameraPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cameraPoint.z = 0;
            if(gridRect.Contains(cameraPoint)) {
                EditorInput.isOverGrid = true;
                hover.TryEnable();
            }
            else {
                EditorInput.isOverGrid = false;
                hover.TryDisable();
            }
        }
    }

}