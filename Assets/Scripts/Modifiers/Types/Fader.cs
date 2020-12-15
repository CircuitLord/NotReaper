using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace NotReaper.Modifier
{

    public class Fader : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;
        public float amount;

        public Fader(string _type, float _startTick, float _endTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
        }
    }
}
