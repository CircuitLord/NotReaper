using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Grid {


    public class IsHoveringGrid : MonoBehaviour {

        private Bounds gridRect;
        public HoverTarget hover;

        private Camera _camera;
        
        private void Start() {
            var transforms = GetComponentsInChildren<Renderer>(false);
            foreach(Renderer renderer in transforms) {
                gridRect.Encapsulate(renderer.bounds);
            }
            gridRect.size *= 1.1f;
            
            _camera = Camera.main;
        }

        private void Update() {
            Vector3 cameraPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            //cameraPoint.x -= _camera.
            //cameraPoint.y -= transform.position.y - _camera.transform.position.y;

            Vector3 fixedPos = _camera.transform.position;
            fixedPos.y -= 0.5f;

            cameraPoint -= fixedPos;
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