using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReaper.Modifier
{
    public class ColorUpdate : Modifier
    {        
        public string leftHandColor;
        public string rightHandColor;

        public ColorUpdate(string _type, float _startTick, string lhColor, string rhColor)
        {
            type = _type;
            startTick = _startTick;
            leftHandColor = lhColor;
            rightHandColor = rhColor;
        }
    }
}
