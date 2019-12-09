using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NotReaper.Timing {
	public struct CopyContext {
		public float[] bufferData;
		public int bufferChannels;
		public int bufferFreq;
		public int index;

		public float volume;
	}

	public class ClipData {
		public float[] samples;
		public int frequency;
		public int channels;
		public uint currentSample = 0;
		public float pan = 0.0f;

		public void SetSampleFromTime(double time) {
			uint sample = (uint)(time * frequency);
			currentSample = sample << 8;
		}

		public double currentTime {
			get { return (currentSample >> 8) / (double)frequency; }
		}

		public float length {
			get { return (samples.Length / channels) / (float)frequency; }
		}

		public void CopySampleIntoBuffer(CopyContext ctx) {
			uint speed = (uint)frequency * 256 / (uint)ctx.bufferFreq;
			float panClamp = Mathf.Clamp(pan, -1.0f, 1.0f);

			int clipChannel = 0;
			int sourceChannel = 0;
			while (sourceChannel < ctx.bufferChannels) {
				float panAmount = 1.0f;
				if(sourceChannel == 0) {
					panAmount = Math.Max(1.0f - panClamp, 1.0f);
				}
				else if(sourceChannel == 1) {
					panAmount = Math.Max(-1.0f - panClamp, 1.0f);
				}

				ctx.bufferData[ctx.index * ctx.bufferChannels + sourceChannel] += samples[(currentSample >> 8) * channels + clipChannel] * ctx.volume * panAmount;

				sourceChannel++;
				clipChannel++;
				if (clipChannel == channels - 1) clipChannel = 0;
			}

			currentSample += speed;
		}
	};

	[RequireComponent(typeof(AudioSource))]
	public class PrecisePlayback : MonoBehaviour {
		public ClipData song;
		public ClipData preview;
		public ClipData leftSustain;
		public ClipData rightSustain;

		public float leftSustainVolume = 0.0f;
		public float rightSustainVolume = 0.0f;

		public float speed = 1.0f;
		public float volume = 1.0f;

		private double dspStartTime = 0.0f;
		private double songStartTime = 0.0f;

		private int sampleRate = 1;

		//Preview
		private bool playPreview = false;
		private uint currentPreviewSongSampleEnd = 0;

		//Metronome
		private bool playMetronome = false;
		private int metronomeSamplesLeft = 0;
		private float metronomeTickLength = 0.01f;
		private float nextMetronomeTick = 0;

		private bool paused = true;
		[SerializeField] private AudioSource source;

		[SerializeField] private Timeline timeline;

		private void Start() {
			sampleRate = AudioSettings.outputSampleRate;
			source.Play();
		}

		public enum LoadType {
			MainSong,
			LeftSustain,
			RightSustain
		}

		public void LoadAudioClip(AudioClip clip, LoadType type) {
			ClipData data = new ClipData();

			data.samples = new float[clip.samples * clip.channels];
			clip.GetData(data.samples, 0);

			data.frequency = clip.frequency;
			data.channels = clip.channels;

			if(type == LoadType.MainSong) {
				song = data;

				preview = new ClipData();
				preview.samples = song.samples;
				preview.channels = song.channels;
				preview.frequency = song.frequency;
			}
			else if(type == LoadType.LeftSustain) {
				leftSustain = data;
				leftSustainVolume = 0.0f;
			}
			else if(type == LoadType.RightSustain) {
				rightSustain = data;
				rightSustainVolume = 0.0f;
			}

			source.Play();
		}

		public double GetTime() {
			double offsetFromStart = AudioSettings.dspTime - dspStartTime;
			return songStartTime + offsetFromStart;
		}

		public double GetTimeFromCurrentSample() {
			return song.currentTime;
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
			song.SetSampleFromTime(songStartTime);
			leftSustain.SetSampleFromTime(songStartTime);
			rightSustain.SetSampleFromTime(songStartTime);
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

			uint sampleStart = (uint)((midTime - duration / 2) * song.frequency);
			uint sampleEnd = (uint)((midTime + duration / 2) * song.frequency);
			preview.currentSample = sampleStart << 8;
			currentPreviewSongSampleEnd = sampleEnd << 8;
			playPreview = true;
		}

		void OnAudioFilterRead(float[] bufferData, int bufferChannels) {
			CopyContext ctx;
			ctx.bufferData = bufferData;
			ctx.bufferChannels = bufferChannels;
			ctx.bufferFreq = sampleRate;

			if(playPreview) {
				int dataIndex = 0;

				while(dataIndex < bufferData.Length / bufferChannels && (preview.currentSample < currentPreviewSongSampleEnd) && (preview.currentSample >> 8) < song.samples.Length) {
					ctx.index = dataIndex;
					ctx.volume = volume;
					preview.CopySampleIntoBuffer(ctx);
					++dataIndex;
				}

				if(preview.currentSample >= currentPreviewSongSampleEnd || (preview.currentSample >> 8) >= song.samples.Length) {
					playPreview = false;
				}
			}

			if (paused || (song.currentSample >> 8) > song.samples.Length) {
				return;
			}

			//double currentTime = AudioSettings.dspTime - dspStartTime;

			//double samplesPerTick = sampleRate * (GetTempoForTime(ShiftTick(new QNT_Timestamp(0), (float)currentTime)).microsecondsPerQuarterNote / 1000000.0);
			//double sample = AudioSettings.dspTime * sampleRate;
			//int startSample = (int)(currentTime * song.frequency * song.channels);
			//int startSample = (int)(currentTime * sampleRate * song.channels);

			for (int dataIndex = 0; dataIndex < bufferData.Length / bufferChannels; dataIndex++) {
				ctx.volume = volume;
				ctx.index = dataIndex;
				song.CopySampleIntoBuffer(ctx);

				if(leftSustain != null) {
					ctx.volume = leftSustainVolume;
					leftSustain.CopySampleIntoBuffer(ctx);
				}

				if(rightSustain != null) {
					ctx.volume = rightSustainVolume;
					rightSustain.CopySampleIntoBuffer(ctx);
				}

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