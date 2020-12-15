using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotReaper.Modifier
{  
    [Serializable]
    public class AimAssistChange : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;
        public float amount;

        public AimAssistChange(string _type, float _startTick, float _endTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
        }
    }
}


