using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class SpeedChange : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;
        public float amount;

        public SpeedChange(string _type, float _startTick, float _endTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
        }
    }
}
