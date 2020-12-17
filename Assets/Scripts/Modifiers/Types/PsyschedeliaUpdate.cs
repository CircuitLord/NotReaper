using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReaper.Modifier
{
    public class PsychedeliaUpdate : Modifier
    {

        public PsychedeliaUpdate(string _type, float _startTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            amount = _amount;
        }
    }
}
