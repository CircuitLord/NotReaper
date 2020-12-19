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
    public class ArenaBrightness : Modifier
    {
        public bool continuous;
        public bool strobo;      

        public ArenaBrightness(string _type, float _startTick, float _endTick, float _amount, bool _continuous, bool _strobo)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
            continuous = _continuous;
            strobo = _strobo;
        }
       
    }
}
