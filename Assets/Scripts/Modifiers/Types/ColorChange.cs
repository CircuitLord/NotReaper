using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{
    [Serializable]
    public class ColorChange : Modifier
    {       
        public float[] leftHandColor = new float[] { 0f, 0f, 0f };
        public float[] rightHandColor = new float[] { 0f, 0f, 0f };

        public ColorChange(string _type, float _startTick, float _endTick, float[] lhColor, float[] rhColor)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            leftHandColor = lhColor;
            rightHandColor = rhColor;
        }
    }
}
