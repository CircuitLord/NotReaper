using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{
    [Serializable]
    public class ZOffset : Modifier
    {
        public float transitionNumberOfTargets;

        public ZOffset(string _type, float _startTick, float _endTick, float _amount, float _transitionNumberOfTargets)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
            transitionNumberOfTargets = _transitionNumberOfTargets;
        }
    }
}
