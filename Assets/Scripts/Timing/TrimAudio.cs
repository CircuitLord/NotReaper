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

        private string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG/ffmpeg.exe");
        Process ffmpeg = new Process();

        public TrimAudio() {
			ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.FileName = ffmpegPath;
        }

        public void SetAudioLength(string path, string output, int offset, double tempo) {
            
            //If we want to trim off 
            if (offset < 0) {

                double ms = Math.Abs(TicksToMS(offset, tempo));
                string newStartTime = new TimeSpan(0, 0, 0, 0, (int)ms).ToString();

                ffmpeg.StartInfo.Arguments = String.Format("-loglevel panic -y -i {0} -ss {1} {2}", path, newStartTime, output);

                ffmpeg.Start();



            }

            else if (offset >= 0) {
                
                double ms = Math.Abs(TicksToMS(offset, tempo));

                string test = String.Format("-y -i {0} -af \"adelay={1}|{1}\" {2}", path, ms, output);

                ffmpeg.StartInfo.Arguments = String.Format("-y -i {0} -af adelay={1}|{1} {2}", path, (int)ms, output);

                ffmpeg.Start();

            }

    
        }


        private double TicksToMS(double offset, double tempo) {
            return offset / 480 * tempo;
        }

    }

}