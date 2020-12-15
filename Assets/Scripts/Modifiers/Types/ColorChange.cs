using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class ColorChange : Modifier
    {       
        public string type;
        public float startTick;
        public string leftHandColor;
        public string rightHandColor;

        public ColorChange(string _type, float _startTick, string lhColor, string rhColor)
        {
            type = _type;
            startTick = _startTick;
            leftHandColor = lhColor;
            rightHandColor = rhColor;
        }
    }
}
