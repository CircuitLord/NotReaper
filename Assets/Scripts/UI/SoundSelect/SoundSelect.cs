using System.Collections;
using System.Collections.Generic;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI {

    public enum UITargetVelocity {
        Standard = 0, Snare = 1, Percussion = 2, ChainStart = 3, Chain = 4, Melee = 5, Metronome = 6, Mine = 7
    }
    public class SoundSelect : MonoBehaviour {
        // Start is called before the first frame update

        public Image ddBG;
        public Image arrow;

        public EditorInput editorInput;


        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void LoadUIColors() {
            Color rColor = NRSettings.config.rightColor;
            ddBG.color = new Color(rColor.r, rColor.g, rColor.b, 0.9f);

            arrow.color = rColor;
        }

        public void ValueChanged(int value) {


            editorInput.SelectVelocity((UITargetVelocity)value);
        }


        


    }

}