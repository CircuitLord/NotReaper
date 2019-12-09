using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NotReaper.Timing {
	[RequireComponent(typeof(AudioSource))]
	public class PrecisePlayback : MonoBehaviour {
		[SerializeField] private AudioSource source;

		private double dspStartTime = 0.0f;
		private double songStartTime = 0.0f;

		private int sampleRate = 1;
		private int songFrequency;
		private int songChannels;
		private float[] songSamples;
		private uint currentSongSample = 0;

		//Preview
		private bool playPreview = false;
		private uint currentPreviewSongSample = 0;
		private uint currentPreviewSongSampleEnd = 0;

		//Metronome
		private bool playMetronome = false;
		private int metronomeSamplesLeft = 0;
		private float metronomeTickLength = 0.01f;
		private float nextMetronomeTick = 0;

		private bool paused = true;

		public Timeline timeline;

		private void Start() {
			sampleRate = AudioSettings.outputSampleRate;
			source.Play();
		}

		public void LoadAudioClip(AudioClip clip) {
			songSamples = new float[clip.samples * clip.channels];
			clip.GetData(songSamples, 0);

			songFrequency = clip.frequency;
			songChannels = clip.channels;
		}

		public double GetTime() {
			double offsetFromStart = AudioSettings.dspTime - dspStartTime;
			return songStartTime + offsetFromStart;
		}

		public double GetTimeFromCurrentSample() {
			return (currentSongSample >> 8) / (double)songFrequency;
		}

		public void StartMetronome() {
			playMetronome = true;
			nextMetronomeTick = 0;
		}

		public void StopMetronome() {
			playMetronome = false;
		}

		public void Play(QNT_Timestamp time) {
			songStartTime = timeline.TimestampToSeconds(time);
			uint sample = (uint)(songStartTime * songFrequency);
			currentSongSample = sample << 8;
			paused = false;
			dspStartTime = AudioSettings.dspTime;
		}

		public void Stop() {
			paused = true;
			StopMetronome();
		}

		public void PlayPreview(QNT_Timestamp time, QNT_Duration previewDuration) {
			float midTime = timeline.TimestampToSeconds(time);
			float duration = Conversion.FromQNT(new Relative_QNT((long)previewDuration.tick), timeline.GetTempoForTime(time).microsecondsPerQuarterNote);
			duration = Math.Min(duration, 0.1f);

			uint sampleStart = (uint)((midTime - duration / 2) * songFrequency);
			uint sampleEnd = (uint)((midTime + duration / 2) * songFrequency);
			currentPreviewSongSample = sampleStart << 8;
			currentPreviewSongSampleEnd = sampleEnd << 8;
			playPreview = true;
		}

		void OnAudioFilterRead(float[] bufferData, int bufferChannels) {
			uint speed = (uint)songFrequency * 256 / (uint)sampleRate;

			if(playPreview) {
				int dataIndex = 0;

				while(dataIndex < bufferData.Length / bufferChannels && (currentPreviewSongSample < currentPreviewSongSampleEnd) && (currentPreviewSongSample >> 8) < songSamples.Length) {
					int clipChannel = 0;
					int sourceChannel = 0;
					while (sourceChannel < bufferChannels) {
						bufferData[dataIndex * bufferChannels + sourceChannel] += songSamples[(currentPreviewSongSample >> 8) * songChannels + clipChannel];

						sourceChannel++;
						clipChannel++;
						if (clipChannel == songChannels - 1) clipChannel = 0;
					}

					currentPreviewSongSample += speed;
					++dataIndex;
				}

				if(currentPreviewSongSample >= currentPreviewSongSampleEnd || (currentPreviewSongSample >> 8) >= songSamples.Length) {
					playPreview = false;
				}
			}

			if (paused || (currentSongSample >> 8) > songSamples.Length) {
				return;
			}

			//double currentTime = AudioSettings.dspTime - dspStartTime;

			//double samplesPerTick = sampleRate * (GetTempoForTime(ShiftTick(new QNT_Timestamp(0), (float)currentTime)).microsecondsPerQuarterNote / 1000000.0);
			//double sample = AudioSettings.dspTime * sampleRate;
			//int startSample = (int)(currentTime * songFrequency * songChannels);
			//int startSample = (int)(currentTime * sampleRate * songChannels);

			for (int dataIndex = 0; dataIndex < bufferData.Length / bufferChannels; dataIndex++) {

				int clipChannel = 0;
				int sourceChannel = 0;
				while (sourceChannel < bufferChannels) {
					bufferData[dataIndex * bufferChannels + sourceChannel] += songSamples[(currentSongSample >> 8) * songChannels + clipChannel];

					sourceChannel++;
					clipChannel++;
					if (clipChannel == songChannels - 1) clipChannel = 0;
				}

				currentSongSample += speed;
				if(playMetronome) {
					if(GetTimeFromCurrentSample() > nextMetronomeTick) {
						metronomeSamplesLeft = (int)(sampleRate * metronomeTickLength);
						nextMetronomeTick = 0;
					}

					if(nextMetronomeTick == 0) {
						QNT_Timestamp nextBeat = timeline.GetClosestBeatSnapped(timeline.ShiftTick(new QNT_Timestamp(0), (float)GetTimeFromCurrentSample()) + Constants.QuarterNoteDuration, 4);
						nextMetronomeTick = timeline.TimestampToSeconds(nextBeat);
					}
				}

				if(metronomeSamplesLeft > 0) {
					const uint MetronomeTickFreq = 817;
					const uint BigMetronomeTickFreq = 1024;

					uint tickFreq = 0;
					QNT_Timestamp thisBeat = timeline.GetClosestBeatSnapped(timeline.ShiftTick(new QNT_Timestamp(0), (float)GetTimeFromCurrentSample()), 4);
					QNT_Timestamp wholeBeat = timeline.GetClosestBeatSnapped(timeline.ShiftTick(new QNT_Timestamp(0), (float)GetTimeFromCurrentSample()), 1);
					if(thisBeat.tick == wholeBeat.tick) {
						tickFreq = BigMetronomeTickFreq;
					}
					else {
						tickFreq = MetronomeTickFreq;
					}

					for(int c = 0; c < bufferChannels; ++c) {
						int totalMetroSamples = (int)(sampleRate * metronomeTickLength);
						float metro = Mathf.Sin(Mathf.PI * 2 * tickFreq * ((totalMetroSamples - metronomeSamplesLeft) / (float)totalMetroSamples) * metronomeTickLength) * 0.33f;
						bufferData[dataIndex * bufferChannels + c] = Mathf.Clamp(bufferData[dataIndex * bufferChannels + c] + metro, -1.0f, 1.0f);
					}
					metronomeSamplesLeft -= 1;
				}
			}
		}
	}
}