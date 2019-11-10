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

        Process ffmpeg = new Process();

        public TrimAudio() {
            string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFMPEG", "ffmpeg.exe");
			ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			ffmpeg.StartInfo.FileName = ffmpegPath;
            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
        }

        public void SetAudioLength(string path, string output, int offset, double bpm, bool skipRetime = false) {

            string log = "";
            double offsetMs = TicksToMs(offset, bpm);
            double magicOctoberOffsetFix = 25.0f;
            double ms = Math.Abs(GetOffsetMs(offsetMs, bpm)) - magicOctoberOffsetFix;

            string args;
            
            if (skipRetime) {
	            args = String.Format("-y -i \"{0}\" -map 0:a \"{1}\"", path, output);
            }

            else {
				args = String.Format("-y -i \"{0}\" -af \"adelay={1}|{1}\" -map 0:a \"{2}\"", path, ms, output);
	            
            }
            
            Debug.Log($"Running ffmpeg with args {args}");

            ffmpeg.StartInfo.Arguments = args;
            ffmpeg.Start();

            Debug.Log(ffmpeg.StandardOutput.ReadToEnd());
            ffmpeg.WaitForExit();
        }

        private double TicksToMs(double offset, double tempo) {
            double beatLength = 60000 / tempo;
            return (offset / 480.0) * beatLength;
        }

        private double GetOffsetMs(double offset, double bpm) {
            double beatLength = 60000 / bpm;
            return GetOffset(offset, beatLength);
        }

        private double GetOffset(double offset, double beatlength) {
            var cappedOffset = offset % beatlength;
            var padding = beatlength * 3;
            var shift = cappedOffset - beatlength;
            return padding - shift;
        }

    }

}