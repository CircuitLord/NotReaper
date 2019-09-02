using System.Collections;
using System.Collections.Generic;
using NotReaper;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {

    //Supplies the timeline with the notes the user can see.
    public class SelectableNotes : MonoBehaviour {

        private void OnTriggerEnter(Collider other) {
            //GridTarget target = other.GetComponentInChildren<GridTarget>();
            //FIXME: Selectable notes loading
            //if (target) {
            //    Timeline.AddSelectableNote(target);
            //}


        }

        private void OnTriggerExit(Collider other) {
            //GridTarget target = other.GetComponentInChildren<GridTarget>();
            //if (target) {
            //    Timeline.RemoveSelectableNote(target);
            //}
        }

    }
}