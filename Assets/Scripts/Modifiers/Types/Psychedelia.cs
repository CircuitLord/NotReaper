using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace NotReaper.Modifier
{
    public class Psychedelia : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;
        public float amount;

        public Psychedelia(string _type, float _startTick, float _endTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
        }

    }
}
