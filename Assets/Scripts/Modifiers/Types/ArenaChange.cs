using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace NotReaper.Modifier
{

    public class ArenaChange : Modifier
    {        
        public ModifierHandler.ModifierType type;
        public float startTick;
        public float endTick;
        public string option1;
        public string option2;

        public ArenaChange(ModifierHandler.ModifierType _type, float _startTick, float _endTick, float _amount, string value1, string value2)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            option1 = value1;
            option2 = value2;
        }
    }
}
