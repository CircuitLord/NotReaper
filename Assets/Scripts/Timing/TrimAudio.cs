using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NotReaper.Timing {

    public class TrimAudio {


        //So I take the offset + 1920 ticks, and convert that to MS based on bpm

        //Tick to MS = ["tick"] / 480 * tempo / 1000000
        //ms to tick = MS * (1920 / (60000 / BPM * 4))

        //positive offset = add silence
        //negative offset = trim the song

        //private string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG/ffmpeg.exe");
        Process ffmpeg = new Process();

        public TrimAudio() {
            string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpeg.exe");
			ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.FileName = ffmpegPath;
            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
        }

        public void SetAudioLength(string path, string output, int offset, double tempo) {

            string log = "";


            double offsetInMS = TicksToMS(offset, tempo);
            
            double ms = Math.Abs(GettingOffsetThing(offsetInMS, tempo));
            
            Debug.Log(path);

            //path = path.Replace(@"'", @"\'");
            
            Debug.Log(path);
            
            UnityEngine.Debug.Log(String.Format("-y -i \"{0}\" -af \"adelay={1}|{1}\" -map 0:a \"{2}\"", path, ms, output));

            ffmpeg.StartInfo.Arguments = String.Format("-y -i \"{0}\" -af \"adelay={1}|{1}\" -map 0:a \"{2}\"", path, ms, output);




            ffmpeg.Start();

            string logStuff = ffmpeg.StandardOutput.ReadToEnd();  

            UnityEngine.Debug.Log(logStuff);

            ffmpeg.WaitForExit();

    
        }



        private double TicksToMS(double offset, double tempo) {
            double beatLength = 60000 / tempo;
            return (offset / 480.0) * beatLength;
 
        }

        private double GettingOffsetThing(double offset, double tempo) {
            //return offset / 480 * tempo;
            double beatLength = 60000 / tempo;
            return GetOffset(offset, beatLength);

        }

        private double GetOffset(double offset, double beatlength) {
            return (beatlength * 2) + (offset - (offset + (offset % beatlength - beatlength))) + beatlength;
        }

    }

}