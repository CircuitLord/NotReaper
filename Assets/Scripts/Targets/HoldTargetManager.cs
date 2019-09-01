using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Targets {


    public class HoldTargetManager : MonoBehaviour {


        [SerializeField] private Transform timelineNotes;
        //public InputField length;
        public GameObject endMarkerPrefab;
        private GameObject endMarker;
        private GameObject endMarkerTl;

        private float PlacementBeatTime = 0;

        private GridTarget parentTarget;

        public float sustainLength = 480;


        void Start() {
            parentTarget = gameObject.GetComponentsInParent<GridTarget>() [0];

            endMarker = Instantiate(endMarkerPrefab, gameObject.transform.position + new Vector3(0, 0, sustainLength / 480f), Quaternion.identity, Timeline.gridNotesStatic);

            endMarkerTl = Instantiate(endMarkerPrefab, new Vector3(0, 0, 0), Quaternion.identity, Timeline.timelineNotesStatic);
            endMarkerTl.transform.localScale = new Vector3(.3f, .3f, .3f);
            endMarkerTl.transform.localPosition = new Vector3(0, 0, 0);

            endMarker.SetActive(true);
            endMarkerTl.SetActive(true);
        }

        // Update is called once per frame
        void Update() {


            endMarker.transform.position = new Vector3(endMarker.transform.position.x, endMarker.transform.position.y, gameObject.transform.position.z + sustainLength); //int.Parse(length.text) / 480f);
            endMarkerTl.transform.localScale = new Vector3(.3f, .3f, .3f);
            //endMarkerTl.transform.position = new Vector3(parentTarget.transform.position.z + parentTarget.beatLength, endMarkerTl.transform.position.y, endMarkerTl.transform.position.z);

            if (gameObject.transform.position.z == 0) {
                //length.gameObject.SetActive(true);
            } else {

                //length.gameObject.SetActive(false);
            }
        }

        void OnDestroy() {

            Destroy(endMarker);
            Destroy(endMarkerTl);
        }

    }

}