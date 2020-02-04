using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Timing;

namespace NotReaper.Targets {


    public class HoldTargetManager : MonoBehaviour {


        [SerializeField] private Transform timelineNotes;
        //public InputField length;
        public GameObject endMarkerPrefab;
        public GameObject endMarker;
        private GameObject endMarkerTl;

        public GameObject sustainButtons;

        private float PlacementBeatTime = 0;

        private TargetIcon parentIcon;

        public QNT_Duration sustainLength = Constants.QuarterNoteDuration;


        public void EnableSustainButtons() {
            sustainButtons.SetActive(true);
        }

        public void DisableSustainButtons() {
            sustainButtons.SetActive(false);

        }


        public void LoadSustainController() {
            parentIcon = gameObject.GetComponentsInParent<TargetIcon>()[0];

            if(endMarkerPrefab != null) {
                endMarker = Instantiate(endMarkerPrefab, gameObject.transform.position + new Vector3(0, 0, sustainLength.ToBeatTime()), Quaternion.identity, Timeline.gridNotesStatic);

                endMarker.SetActive(true);

                parentIcon.holdEndTrans = endMarker.transform;
            }
        }


        public void ApplyNewLineScale() {

        }


        public void UpdateSustainLength(QNT_Duration newLength) {
            sustainLength = newLength;

            if(endMarker != null) {
                endMarker.transform.position = new Vector3(endMarker.transform.position.x, endMarker.transform.position.y, gameObject.transform.position.z + sustainLength.ToBeatTime());
            }
        }

        /// <summary>
        /// Called when the user clicks on the arrows to change the sustain length. True is increase, false is decrease.
        /// </summary>
        public event Action<bool> OnTryChangeSustainEvent;

        public void IncreaseSustainLength() {
            OnTryChangeSustainEvent(true);
        }

        public void DecreaseSustainLength() {
            OnTryChangeSustainEvent(false);
        }


        // Update is called once per frame
        //void Update() {


            //endMarker.transform.position = new Vector3(endMarker.transform.position.x, endMarker.transform.position.y, gameObject.transform.position.z + sustainLength / 480f); //int.Parse(length.text) / 480f);
            //endMarkerTl.transform.localScale = new Vector3(.3f, .3f, .3f);
            //endMarkerTl.transform.position = new Vector3(parentIcon.transform.position.z + parentIcon.beatLength, endMarkerTl.transform.position.y, endMarkerTl.transform.position.z);

            //if (gameObject.transform.position.z == 0) {
                //length.gameObject.SetActive(true);
            //} else {

                //length.gameObject.SetActive(false);
            //}
        //}

        void OnDestroy() {
            Destroy(endMarker);
            Destroy(endMarkerTl);
        }

    }

}