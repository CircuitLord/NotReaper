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
        public string leftHandColor;
        public string rightHandColor;

        public ColorChange(string _type, float _startTick, float _endTick, string lhColor, string rhColor)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            leftHandColor = lhColor;
            rightHandColor = rhColor;
        }
    }
}
