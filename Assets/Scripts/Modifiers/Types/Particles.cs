using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReaper.Modifier
{
    public class Particles : Modifier
    {

        public Particles(string _type, float _startTick, float _endTick, float _amount)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
            amount = _amount;
        }
    }
}
