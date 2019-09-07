using System.Collections;
using System.Collections.Generic;
using NotReaper.Targets;
using UnityEngine;
using UnityEngine.EventSystems;
using NotReaper.UserInput;

namespace NotReaper.Grid {


    public enum SnappingMode { None, Grid, Melee }

    public class NoteGridSnap : MonoBehaviour {


        [SerializeField] public float xSize;
        [SerializeField] public float ySize;

        private SnappingMode snapMode = SnappingMode.Grid;

        public Transform ghost;
        public GameObject standardGrid;
        public GameObject meleeGrid;

        


        private void Awake() {
            SetSnappingMode(SnappingMode.Grid);
        }

        private void Update() {
            if (EditorInput.isOverGrid) {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                ghost.position = SnapToGrid(mousePos);

            }
            HandleKeybinds();

        }


        private void HandleKeybinds() {
            if (Input.GetKeyDown(KeyCode.G)) {
                SetSnappingMode(SnappingMode.Grid);
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                SetSnappingMode(SnappingMode.Melee);
            }
            if (Input.GetKeyDown(KeyCode.N)) {
                SetSnappingMode(SnappingMode.None);
            }

        }


        public void EnableHoverTarget() {
            ghost.gameObject.SetActive(true);
        }

        public void DisableHoverTarget() {
            ghost.gameObject.SetActive(false);
        }

        private void OnMouseEnter() {
            EnableHoverTarget();
            EditorInput.isOverGrid = true;
            
        }

        private void OnMouseExit() {
            DisableHoverTarget();
            EditorInput.isOverGrid = false;
        }


        public void SetSnappingMode(SnappingMode s) {
            snapMode = s;
            meleeGrid.SetActive(snapMode == SnappingMode.Melee);
            standardGrid.SetActive(snapMode == SnappingMode.Grid);
        }


        public Vector2 GetNearestPointOnGrid(Vector2 pos) {

            //pos -= gridOffset; //Enable if grid is actually offset.
            pos.y += 0.45f;
            int x = Mathf.FloorToInt(pos.x / NotePosCalc.xSize);
            int y = Mathf.FloorToInt(pos.y / NotePosCalc.ySize);

            Vector2 result = new Vector2((float) x * NotePosCalc.xSize, (float) y * NotePosCalc.ySize);
            result.x += NotePosCalc.xSize / 2; //0.65f; //from 1.3 / 2

            //result += gridOffset; //Enable if grid is actually offset.

            return result;
        }

        public Vector3 SnapToGrid(Vector3 pos) {
            switch (snapMode) {
                case SnappingMode.Grid:
                    return GetNearestPointOnGrid(pos);
                case SnappingMode.Melee:
                    return new Vector3(Mathf.Sign(pos.x * NotePosCalc.xSize) * 2, Mathf.Sign(pos.y * NotePosCalc.ySize), pos.z + 5);
            }
            return new Vector3(pos.x, pos.y, pos.z + 5);
        }


    }


}