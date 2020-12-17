using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class ClickNotifier : MonoBehaviour
    {
        private TimelineEntry entry;

        public void SetEntry(GameObject startMark)
        {
            entry = startMark.GetComponent<TimelineEntry>();
        }
        private void OnMouseDown()
        {
            if (entry is null) return;
            entry.ReportClick();
        }
    }
}

