using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace NotReaper.Modifier
{
    
    public class ArenaRotation : Modifier
    {       
        public string type;
        public float startTick;
        public float endTick;
        public float amount;
        public bool continuous;

        public ArenaRotation(string _type, float _startTick, float _endTick, float _amount, bool _continuous)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
            continuous = _continuous;
        }
    }
}
