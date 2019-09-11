using System.Collections;
using System.Collections.Generic;
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


        private void Update() {
            
            if (!iconEnabled) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            switch (EditorInput.selectedTool) {
                case EditorTool.ChainBuilder:
                case EditorTool.DragSelect:

                    transform.position = new Vector3(mousePos.x, mousePos.y, -1f);

                    break;

                default:
                    transform.position = NoteGridSnap.SnapToGrid(new Vector3(mousePos.x, mousePos.y, -1f), EditorInput.selectedSnappingMode);
                    break;
            }





        }


        public void UpdateUIHandColor(Color color) {
            standard.GetComponent<SpriteRenderer>().color = color;
            hold.GetComponent<SpriteRenderer>().color = color;
            horizontal.GetComponent<SpriteRenderer>().color = color;
            vertical.GetComponent<SpriteRenderer>().color = color;
            chainStart.GetComponent<SpriteRenderer>().color = color;
            chainNode.GetComponent<SpriteRenderer>().color = color;
            melee.GetComponent<SpriteRenderer>().color = color;
            cursorTint.color = color;
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
        }


    }

}