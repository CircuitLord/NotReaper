using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using UnityEngine;

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
            string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG/ffmpeg.exe");
			ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.FileName = ffmpegPath;
            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        }

        public void SetAudioLength(string path, string output, int offset, double tempo) {
            
            //If we want to trim off 
            //if (offset < 0) {
           // if (false) {

             //   double ms = Math.Abs(GettingOffsetThing(offset, tempo));
              //  string newStartTime = new TimeSpan(0, 0, 0, 0, (int)ms).ToString();

               // ffmpeg.StartInfo.Arguments = String.Format("-loglevel panic -y -i \"{0}\" -ss {1} -map 0:a \"{2}\"", path, newStartTime, output);

               // ffmpeg.Start();

              //  ffmpeg.WaitForExit();



           // }

           // else {  

                double offsetInMS = TicksToMS(offset, tempo);
                
                double ms = Math.Abs(GettingOffsetThing(offsetInMS, tempo));

                ffmpeg.StartInfo.Arguments = String.Format("-y -i \"{0}\" -af \"adelay={1}|{1}\" -map 0:a \"{2}\"", path, ms, output);

                ffmpeg.Start();

                ffmpeg.WaitForExit();

           // }

    
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