using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NotReaper.Models
{
    public class MoggSong
    {
        public MoggVol volume;
        public string moggPath;
        public MoggVol pan;
        public string[] moggString;

        public MoggSong(MemoryStream ms)
        {
            StreamReader reader = new StreamReader(ms);
            moggString = Encoding.UTF8.GetString(ms.ToArray()).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in moggString)
            {
                if (line.Contains("(vol")) this.volume = GetMoggVolFromLine(line);
                if (line.Contains("(pans")) this.volume = GetMoggVolFromLine(line);
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

        public string ExportToText()
        {
            string[] exportString = moggString;
            int volIndex = 0;
            int panIndex = 0;
            for (int i = 0; i < exportString.Length; i++)
            {
                exportString[i].Replace("\n", "");
                if (exportString[i].Contains("(vols")) volIndex = i;
                if (exportString[i].Contains("(pan")) panIndex = i;
            }
            exportString[volIndex] = $"(vols ({volume.l.ToString("n2")}   {volume.r.ToString("n2")}))";
            exportString[panIndex] = $"(pans ({pan.l.ToString("n2")}   {pan.l.ToString("n2")}))";
            return string.Join(Environment.NewLine, exportString);
        }

        public void SetVolume(float value)
        {
            this.volume.l = this.volume.r = value;
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
