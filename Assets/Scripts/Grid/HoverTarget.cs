using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;

namespace NotReaper.Grid {


    public class HoverTarget : MonoBehaviour {

        public GameObject icon;
        private bool iconEnabled = true;
        public GameObject cursor;
        public SpriteRenderer cursorTint;
        public GameObject standard;
        public GameObject hold;
        public GameObject horizontal;
        public GameObject vertical;
        public GameObject chainStart;
        public GameObject chainNode;
        public GameObject melee;
        public GameObject mine;

        private SpriteRenderer srstandard;
        private SpriteRenderer srhold;
        private SpriteRenderer srhorizontal;
        private SpriteRenderer srvertical;
        private SpriteRenderer srchainstart;
        private SpriteRenderer srchainnode;
        private SpriteRenderer srmelee;
        private SpriteRenderer srmine;


        private void Start() {
            srstandard = standard.GetComponent<SpriteRenderer>();
            srhold = hold.GetComponent<SpriteRenderer>();
            srhorizontal = horizontal.GetComponent<SpriteRenderer>();
            srvertical = vertical.GetComponent<SpriteRenderer>();
            srchainstart= chainStart.GetComponent<SpriteRenderer>();
            srchainnode = chainNode.GetComponent<SpriteRenderer>();
            srmelee = melee.GetComponent<SpriteRenderer>();
            srmine = mine.GetComponent<SpriteRenderer>();
        }


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

        Vector3 lastPos = new Vector3();
        private void Update() {
            
            if (!iconEnabled) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

            srstandard.DOColor(color, animColorSpeed);
            srhold.DOColor(color, animColorSpeed);
            srhorizontal.DOColor(color, animColorSpeed);
            srvertical.DOColor(color, animColorSpeed);
            srchainstart.DOColor(color, animColorSpeed);
            srchainnode.DOColor(color, animColorSpeed);
            srmelee.DOColor(color, animColorSpeed);
            srmine.DOColor(color, animColorSpeed);
            cursorTint.DOColor(color, animColorSpeed);
        }

        public void UpdateUITool(EditorTool tool) {

            if (tool == EditorTool.DragSelect || tool == EditorTool.ChainBuilder) {
                cursor.SetActive(true);
            } else {
                cursor.SetActive(false);
            }

            standard.SetActive(tool == EditorTool.Standard);
            hold.SetActive(tool == EditorTool.Hold);
            horizontal.SetActive(tool == EditorTool.Horizontal);
            vertical.SetActive(tool == EditorTool.Vertical);
            chainStart.SetActive(tool == EditorTool.ChainStart);
            chainNode.SetActive(tool == EditorTool.ChainNode);
            melee.SetActive(tool == EditorTool.Melee);
            mine.SetActive(tool == EditorTool.Mine);
        }


    }

}