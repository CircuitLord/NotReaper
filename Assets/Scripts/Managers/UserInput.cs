using System.Collections;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Grid.SelectTool;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.Managers {


    public class UserInput : MonoBehaviour {

        private float mouseChargeLevel = 0f;
        [SerializeField] private int mouseChargeSpeed = 100;
        private bool mouseButtonHeld = false;
        private Vector2 mouseDragPosStart;
        //[SerializeField] private Timeline timeline;
        [SerializeField] private Toggle L;
        [SerializeField] private Toggle R;
        [SerializeField] private OptionsMenu optionsMenu;
        [SerializeField] private Toggle standardButton;
        [SerializeField] private Toggle holdButton;
        [SerializeField] private Toggle horzButton;
        [SerializeField] private Toggle vertButton;
        [SerializeField] private Toggle chainStartButton;
        [SerializeField] private Toggle chainPartButton;
        [SerializeField] private Toggle meleeButton;


        private void Start() {
            //default selecitons
            L.isOn = true;
            standardButton.isOn = true;
        }


        void Update() {

            //Doesn't need to use the input manager, no reason to change.
            if (Input.GetMouseButtonDown(0)) {
                //mouseDragPosStart = Input.mousePosition;
            }


            if (Input.GetMouseButton(0)) {
                //mouseChargeLevel += Time.deltaTime * mouseChargeSpeed;
            }
            if (Input.GetMouseButtonUp(0)) {
                mouseChargeLevel = 0f;

                if (mouseButtonHeld == true) {
                    mouseButtonHeld = false;
                    //TODO: Move to notegrid code
                    var bounds = DragUtil.GetViewportBounds(Camera.main, mouseDragPosStart, Input.mousePosition);

                    //TODO: add the notes inside bounding box


                }
            }


            //Drag square
            if (mouseChargeLevel > UserPrefsManager.mouseHoldForDrag) {
                //mouseButtonHeld = true;
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                //optionsMenu.SelectStandard();
                //standardButton.isOn = true;
                Timeline.TimelineStatic.LoadAudicaFile();
            }


            if (Input.GetKeyDown(InputManager.IM.selectStandard)) {
                //optionsMenu.SelectStandard();
                standardButton.isOn = true;
                //UserPrefsManager.ChangeColors();
            }
            if (Input.GetKeyDown(InputManager.IM.selectHold)) {
                holdButton.isOn = true;
            }
            if (Input.GetKeyDown(InputManager.IM.selectHorz)) {
                horzButton.isOn = true;
            }
            if (Input.GetKeyDown(InputManager.IM.selectVert)) {
                vertButton.isOn = true;
            }
            if (Input.GetKeyDown(InputManager.IM.selectChainStart)) {
                chainStartButton.isOn = true;
            }
            if (Input.GetKeyDown(InputManager.IM.selectChainNode)) {
                chainPartButton.isOn = true;
            }
            if (Input.GetKeyDown(InputManager.IM.selectMelee)) {
                meleeButton.isOn = true;
            }

            if (Input.GetKeyDown(InputManager.IM.toggleColor)) {

                if (L.isOn == false) {
                    L.isOn = true;
                } else if (R.isOn == false) {
                    R.isOn = true;
                } else {
                    L.isOn = true;
                }

            }

            if (Input.GetKeyDown(InputManager.IM.selectSoundKick)) {
                //soundSelect.value = 0;
            }
            if (Input.GetKeyDown(InputManager.IM.selectSoundSnare)) {
                //soundSelect.value = 1;
            }
            if (Input.GetKeyDown(InputManager.IM.selectSoundPercussion)) {
                //soundSelect.value = 2;
            }
            if (Input.GetKeyDown(InputManager.IM.selectSoundChainStart)) {
                //soundSelect.value = 3;
            }
            if (Input.GetKeyDown(InputManager.IM.selectSoundChainNode)) {
                //soundSelect.value = 4;
            }
            if (Input.GetKeyDown(InputManager.IM.selectSoundMelee)) {
                //soundSelect.value = 5;
            }


        }


        void OnGUI() {
            //Draw the rectangle
            if (mouseButtonHeld) {
                // Create a rect from both mouse positions
                var rect = DragUtil.GetScreenRect(mouseDragPosStart, Input.mousePosition);
                DragUtil.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                DragUtil.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }


    }
}