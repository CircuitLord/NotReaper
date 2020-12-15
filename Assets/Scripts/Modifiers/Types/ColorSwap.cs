using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class ColorSwap : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;

        public ColorSwap(string _type, float _startTick, float _endTick)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
        }

    }
}
