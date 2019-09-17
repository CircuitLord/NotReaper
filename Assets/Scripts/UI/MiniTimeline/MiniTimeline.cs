using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Models;

namespace NotReaper.UI {


    public class MiniTimeline : MonoBehaviour {

        //bar is 440 pixels long total

        public Transform songPreviewIcon;


        public float mouseClickAreaLength = 12.34f;
        public double barLength;

        public Transform bar;

        public Timeline timeline;

        public Transform bookmarksParent;

        public GameObject bookmarkPrefab;

        [HideInInspector] public GameObject[] bookmarks = new GameObject[10];


        [HideInInspector] public bool isMouseOver = false;


        public void SetPercentagePlayed(double percent) {
            double x = barLength * percent;
            x -= barLength / 2;
            bar.localPosition = new Vector3((float)x, 0, 0);
        }

        private void OnMouseDown() {
            var x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            //x -= transform.position.x;

            //-4.7 to 4.7
            //9.4 length
            x += mouseClickAreaLength / 2;
            float percent = x / mouseClickAreaLength;

            timeline.JumpToPercent(percent);
        }


        public void SetSongPreviewPoint(double percent) {
            double x = barLength * percent;
            x -= barLength / 2;
            songPreviewIcon.localPosition = new Vector3((float)x, 0, 0);
        }

        private void OnMouseOver() {
            isMouseOver = true;
        }

        private void OnMouseExit() {
            isMouseOver = false;
        }


        public void JumpToBookmark(int i) {
            if (bookmarks[i] = null) return;
            timeline.JumpToPercent((float)bookmarks[i].GetComponent<Bookmark>().percentBookmark);

        }

        public void SetBookmark(int i) {

            if (bookmarks[i] != null) {
                Destroy(bookmarks[i]);
                bookmarks[i] = null;
            }

            bookmarks[i] = Instantiate(bookmarkPrefab, new Vector3(0, 0, 0), Quaternion.identity, bookmarksParent);

            double percent = timeline.GetPercentagePlayed();
            double x = barLength * percent;
            x -= barLength / 2;

            bookmarks[i].transform.localPosition = new Vector3((float)x, 0, 0);

            bookmarks[i].GetComponent<Bookmark>().SetIndex(i);
            bookmarks[i].GetComponent<Bookmark>().percentBookmark = percent;
        }



        

        private void Update() {

            bool isCtrlDown = false;
            bool isShiftDown = false;


            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                isCtrlDown = true;
            } else {
                isCtrlDown = false;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                isShiftDown = true;
            } else {
                isShiftDown = false;
            }

            //Check for bookmark keypresses if ctrl is down.
            if (isCtrlDown && !isShiftDown) {
                if (Input.GetKeyDown(KeyCode.Alpha0)) JumpToBookmark(0);
                else if (Input.GetKeyDown(KeyCode.Alpha1)) JumpToBookmark(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) JumpToBookmark(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) JumpToBookmark(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4)) JumpToBookmark(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5)) JumpToBookmark(5);
                else if (Input.GetKeyDown(KeyCode.Alpha6)) JumpToBookmark(6);
                else if (Input.GetKeyDown(KeyCode.Alpha7)) JumpToBookmark(7);
                else if (Input.GetKeyDown(KeyCode.Alpha8)) JumpToBookmark(8);
                else if (Input.GetKeyDown(KeyCode.Alpha9)) JumpToBookmark(9);          
            }

            
            //Input stuff:



            if (isShiftDown && isCtrlDown) {
                if (Input.GetKeyDown(KeyCode.Alpha0)) SetBookmark(0);
                else if (Input.GetKeyDown(KeyCode.Alpha1)) SetBookmark(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) SetBookmark(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) SetBookmark(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4)) SetBookmark(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5)) SetBookmark(5);
                else if (Input.GetKeyDown(KeyCode.Alpha6)) SetBookmark(6);
                else if (Input.GetKeyDown(KeyCode.Alpha7)) SetBookmark(7);
                else if (Input.GetKeyDown(KeyCode.Alpha8)) SetBookmark(8);
                else if (Input.GetKeyDown(KeyCode.Alpha9)) SetBookmark(9);   
            }


        

        }



    }

}