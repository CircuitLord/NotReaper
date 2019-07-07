using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper {


    public class BottomTimeline : MonoBehaviour {

        [SerializeField] private Slider timelineBottom;
        [SerializeField] private Timeline timeline;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        //void Update() {
        //timelineBottom.value = timeline.GetPercentagePlayed();
        //}
    }
}