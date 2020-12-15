using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReaper.Modifier
{
    public class HiddenTelegraphs : Modifier
    {
        public string type;
        public float startTick;
        public float endTick;

        public HiddenTelegraphs(string _type, float _startTick, float _endTick)
        {
            type = _type;
            startTick = _startTick;
            endTick = _endTick;
        }
    }
}
