using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.Grid {


    public class HoverTarget : MonoBehaviour {

        public GameObject icon;
        private bool iconEnabled = true;
        public GameObject cursor;
        public Image cursorTint;

        private Camera cam;

        [SerializeField] private Image standard;
        [SerializeField] private Image hold;
        [SerializeField] private Image horizontal;
        [SerializeField] private Image vertical;
        [SerializeField] private Image chainstart;
        [SerializeField] private Image chainnode;
        [SerializeField] private Image melee;
        [SerializeField] private Image mine;



        public void TryEnable() {
            iconEnabled = true;
            icon.SetActive(true);
        }
        public void TryDisable() {

            switch (EditorInput.selectedTool) {
                case EditorTool.ChainBuilder:
                case EditorTool.DragSelect:
                    break;

                default:
                    iconEnabled = false;
                    icon.SetActive(false);
                    break;
            }



        }

        public void Awake()
        {
            cam = Camera.main;
        }

        Vector3 lastPos = new Vector3();
        private void Update() {
            
            if (!iconEnabled) return;

            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            switch (EditorInput.selectedTool) {
                case EditorTool.ChainBuilder:
                case EditorTool.DragSelect:

                    transform.position = new Vector3(mousePos.x, mousePos.y, -1f);

                    break;

                default:
                    Vector3 newPos = NoteGridSnap.SnapToGrid(new Vector3(mousePos.x, mousePos.y, -1f), EditorInput.selectedSnappingMode);
                    //if (newPos == lastPos) {
                        //return;
                    //}
                    //lastPos = newPos;
                    transform.position = newPos;
                    //transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, 0.3f);
                    break;
            }





        }


        public float animColorSpeed = 0.3f;
        public void UpdateUIHandColor(Color color) {

            standard.DOColor(color, animColorSpeed);
            hold.DOColor(color, animColorSpeed);
            horizontal.DOColor(color, animColorSpeed);
            vertical.DOColor(color, animColorSpeed);
            chainstart.DOColor(color, animColorSpeed);
            chainnode.DOColor(color, animColorSpeed);
            melee.DOColor(color, animColorSpeed);
            mine.DOColor(color, animColorSpeed);
            cursorTint.DOColor(color, animColorSpeed);
        }

        public void UpdateUITool(EditorTool tool) {

            if (tool == EditorTool.DragSelect || tool == EditorTool.ChainBuilder) {
                cursor.SetActive(true);
            } else {
                cursor.SetActive(false);
            }

            standard.gameObject.SetActive(tool == EditorTool.Standard);
            hold.gameObject.SetActive(tool == EditorTool.Hold);
            horizontal.gameObject.SetActive(tool == EditorTool.Horizontal);
            vertical.gameObject.SetActive(tool == EditorTool.Vertical);
            chainstart.gameObject.SetActive(tool == EditorTool.ChainStart);
            chainnode.gameObject.SetActive(tool == EditorTool.ChainNode);
            melee.gameObject.SetActive(tool == EditorTool.Melee);
            mine.gameObject.SetActive(tool == EditorTool.Mine);
        }


    }

}