using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class ClickNotifier : MonoBehaviour
    {
        private Modifier modifier;

        public void SetModifier(Modifier startMark)
        {
            modifier = startMark;
        }

        public Modifier GetModifier()
        {
            return modifier;
        }
        public void Click(bool singleSelect)
        {
            if (modifier is null) return;
            modifier.ReportClick(singleSelect);
        }
    }
}

