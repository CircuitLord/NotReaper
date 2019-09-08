using System.Collections;
using System.Collections.Generic;
using NotReaper;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {

    //Supplies the timeline with the notes the user can see.
    public class LoadedNotes : MonoBehaviour {

        private void OnTriggerEnter(Collider other) {
            TargetIcon targetIcon = other.GetComponent<TargetIcon>();
            if (targetIcon) {
                Debug.Log("Note entered: " + targetIcon);
                //Timeline.AddLoadedNote(target);
                targetIcon.IconEnterLoadedNotes();

            }


        }

        private void OnTriggerExit(Collider other) {
            TargetIcon targetIcon = other.GetComponent<TargetIcon>();
            if (targetIcon) {
                targetIcon.IconExitLoadedNotes();
            }
        }

    }
}