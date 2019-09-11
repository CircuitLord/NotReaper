using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using UnityEngine;

namespace NotReaper.Timing {

    public class TrimAudio {

        //Maybe use this?
        void TrimMp3(string inputPath, string outputPath, TimeSpan? begin, TimeSpan? end) {
            if (begin.HasValue && end.HasValue && begin > end)
                throw new ArgumentOutOfRangeException("end", "end should be greater than begin");

            using(var reader = new Mp3FileReader(inputPath))
            using(var writer = File.Create(outputPath)) {
                Mp3Frame frame;
                while ((frame = reader.ReadNextFrame()) != null)
                    if (reader.CurrentTime >= begin || !begin.HasValue) {
                        if (reader.CurrentTime <= end || !end.HasValue)
                            writer.Write(frame.RawData, 0, frame.RawData.Length);
                        else break;
                    }
            }
        }

    }

}