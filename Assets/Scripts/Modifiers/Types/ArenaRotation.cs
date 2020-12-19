using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace NotReaper.Modifier
{
    [Serializable]
    public class ArenaRotation : Modifier
    {       
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
