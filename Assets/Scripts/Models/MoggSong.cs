using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NotReaper.Models
{
    public class MoggSong
    {
        public MoggVol volume;
        public string moggPath;
        public MoggVol pan;

        public MoggSong(MemoryStream ms)
        {
            StreamReader reader = new StreamReader(ms);
            string[] moggString = Encoding.UTF8.GetString(ms.ToArray()).Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            foreach (var line in moggString)
            {
                if (line.Contains("vol")) this.volume = GetMoggVolFromLine(line);
                if (line.Contains("pans")) this.volume = GetMoggVolFromLine(line);
            }
        }

        public MoggVol GetMoggVolFromLine(string line)
        {
            var split = line.Split(new char[] { '(', ')' });
            string[] values;
            if(split[2].Contains("    ")) values = split[2].Split(new string[] { "    " }, StringSplitOptions.None);
            else if(split[2].Contains("   ")) values = split[2].Split(new string[] { "   " }, StringSplitOptions.None);
            else if(split[2].Contains("  ")) values = split[2].Split(new string[] { "  " }, StringSplitOptions.None);
            else values = split[2].Split(new string[] { " " }, StringSplitOptions.None);
            return new MoggVol(float.Parse(values[0]), float.Parse(values[1]));
        }
    }

    public struct MoggVol
    {
        public float l;
        public float r;

        public MoggVol(float l, float r)
        {
            this.l = l;
            this.r = r;
        }
    }
}
