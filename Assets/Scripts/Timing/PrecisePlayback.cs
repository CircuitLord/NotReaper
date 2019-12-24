using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Timing {
	public struct CopyContext {
		public float[] bufferData;
		public int bufferChannels;
		public int bufferFreq;
		public int index;

		public float volume;
		public float playbackSpeed;

		//Write value
		public float outputValue;
	}

	public class ClipData {
		public float[] samples;
		public int frequency;
		public int channels;
		public UInt64 currentSample = 0;
		public uint scaledCurrentSample {
			get { return (uint)(currentSample >> PrecisionShift); }
		}

		public float pan = 0.0f;

		public float duckVolume = 0.0f;

		public const int PrecisionShift = 32;

		public void SetSampleFromTime(double time) {
			UInt64 sample = (uint)(time * frequency);
			currentSample = sample << PrecisionShift;
		}

		public double currentTime {
			get { return (currentSample >> PrecisionShift) / (double)frequency; }
		}

		public float length {
			get { return (samples.Length / channels) / (float)frequency; }
		}

		public void CopySampleIntoBuffer(CopyContext ctx) {
			UInt64 shiftNum = ((UInt64)1 << PrecisionShift);
			UInt64 speed = (UInt64)frequency * shiftNum / (UInt64)ctx.bufferFreq;
			if(ctx.playbackSpeed != 1.0f) {
				speed = (UInt64)(speed * ctx.playbackSpeed);
			}

			float panClamp = Mathf.Clamp(pan, -1.0f, 1.0f);

			int clipChannel = 0;
			int sourceChannel = 0;
			float maxValue = 0.0f;
			while (sourceChannel < ctx.bufferChannels) {
				float panAmount = 1.0f;
				if(sourceChannel == 0) {
					panAmount = Math.Max(1.0f - panClamp, 1.0f);
				}
				else if(sourceChannel == 1) {
					panAmount = Math.Max(-1.0f - panClamp, 1.0f);
				}

				float value = 0.0f;
				long samplePos = (uint)(currentSample >> PrecisionShift) * channels + clipChannel;
				if(samplePos < samples.Length) {
					value = samples[samplePos];
				}

				float duckValue = Mathf.Clamp(1.0f - duckVolume, 0.0f, 1.0f);
				maxValue = Math.Max(value * ctx.volume * duckValue, maxValue);

				ctx.bufferData[ctx.index * ctx.bufferChannels + sourceChannel] += value * ctx.volume * panAmount * duckValue;

				sourceChannel++;
				clipChannel++;
				if (clipChannel >= channels) clipChannel = 0;
			}

			currentSample += speed;
			ctx.outputValue = Math.Abs(maxValue);
		}
	};

	[RequireComponent(typeof(AudioSource))]
	public class PrecisePlayback : MonoBehaviour {
		public ClipData song;
		public ClipData preview;
		public ClipData leftSustain;
		public ClipData rightSustain;

		private ClipData kick;
		private ClipData snare;
		private ClipData percussion;
		private ClipData chainStart;
		private ClipData chainNote;
		private ClipData melee;

		public float leftSustainVolume = 0.0f;
		public float rightSustainVolume = 0.0f;

		public float speed = 1.0f;
		public float volume = 1.0f;
		public float hitSoundVolume = 1.0f;

		private double dspStartTime = 0.0f;
		private double songStartTime = 0.0f;

		private int sampleRate = 1;

		//Preview
		private bool playPreview = false;
		private UInt64 currentPreviewSongSampleEnd = 0;

		//Metronome
		private bool playMetronome = false;
		private int metronomeSamplesLeft = 0;
		private float metronomeTickLength = 0.01f;
		private float nextMetronomeTick = 0;

		private bool paused = true;
		[SerializeField] private AudioSource source;

		[SerializeField] private Timeline timeline;

		[SerializeField] private AudioClip KickClip;
		[SerializeField] private AudioClip SnareClip;
		[SerializeField] private AudioClip PercussionClip;
		[SerializeField] private AudioClip ChainStartClip;
		[SerializeField] private AudioClip ChainNoteClip;
		[SerializeField] private AudioClip MeleeClip;

		private void Start() {
			sampleRate = AudioSettings.outputSampleRate;
			source.Play();
			StartCoroutine(LoadHitsounds());
		}

		IEnumerator LoadHitsounds() {
			while (KickClip.loadState != AudioDataLoadState.Loaded) yield return null;
			while (SnareClip.loadState != AudioDataLoadState.Loaded) yield return null;
			while (PercussionClip.loadState != AudioDataLoadState.Loaded) yield return null;
			while (ChainStartClip.loadState != AudioDataLoadState.Loaded) yield return null;
			while (ChainNoteClip.loadState != AudioDataLoadState.Loaded) yield return null;
			while (MeleeClip.loadState != AudioDataLoadState.Loaded) yield return null;

			kick = FromAudioClip(KickClip);
			snare = FromAudioClip(SnareClip);
			percussion = FromAudioClip(PercussionClip);
			chainStart = FromAudioClip(ChainStartClip);
			chainNote = FromAudioClip(ChainNoteClip);
			melee = FromAudioClip(MeleeClip);
		}

		public enum LoadType {
			MainSong,
			LeftSustain,
			RightSustain
		}

		private ClipData FromAudioClip(AudioClip clip) {
			ClipData data = new ClipData();

			data.samples = new float[clip.samples * clip.channels];
			clip.GetData(data.samples, 0);

			data.frequency = clip.frequency;
			data.channels = clip.channels;

			return data;
		}

		public void LoadAudioClip(AudioClip clip, LoadType type) {
			ClipData data = FromAudioClip(clip);

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
			return song.currentTime;
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
			
			if(leftSustain != null) { 
				leftSustain.SetSampleFromTime(songStartTime);
			}

			if(rightSustain != null) { 
				rightSustain.SetSampleFromTime(songStartTime);
			}

			paused = false;
			clearHitsounds = true;
			dspStartTime = AudioSettings.dspTime;
			source.Play();
		}

		public void Stop() {
			paused = true;
			StopMetronome();
		}

		public void PlayPreview(QNT_Timestamp time, Relative_QNT previewDuration) {
			float midTime = timeline.TimestampToSeconds(time);
			float duration = Conversion.FromQNT(new Relative_QNT(Math.Abs(previewDuration.tick)), timeline.GetTempoForTime(time).microsecondsPerQuarterNote);
			duration = Math.Min(duration, 0.1f);

			UInt64 sampleStart = (UInt64)((midTime - duration / 2) * song.frequency);
			UInt64 sampleEnd = (UInt64)((midTime + duration / 2) * song.frequency);
			preview.currentSample = sampleStart << ClipData.PrecisionShift;
			currentPreviewSongSampleEnd = sampleEnd << ClipData.PrecisionShift;
			playPreview = true;
			source.Play();

			List<HitsoundEvent> previewHits = new List<HitsoundEvent>();
			AddHitsoundEvents(previewHits, time, previewDuration.tick > 0 ? timeline.ShiftTick(time, duration) : time);
			for(int i = 0; i < previewHits.Count; ++i) {
				HitsoundEvent ev = previewHits[i];
				ev.waitSamples = 0;
				previewHits[i] = ev;
			}

			newPreviewHitsoundEvents = previewHits;
		}

		public void PlayHitsound(QNT_Timestamp time) {
			List<HitsoundEvent> previewHits = new List<HitsoundEvent>();
			AddHitsoundEvents(previewHits, time, time);
			for(int i = 0; i < previewHits.Count; ++i) {
				HitsoundEvent ev = previewHits[i];
				ev.waitSamples = 0;
				previewHits[i] = ev;
			}

			newPreviewHitsoundEvents = previewHits;
		}

		struct HitsoundEvent {
			public uint ID;
			public QNT_Timestamp time;
			public UInt64 waitSamples;
			public UInt64 currentSample;
			public ClipData sound;
			public float pan;
			public float volume;
		};
		List<HitsoundEvent> hitsoundEvents = new List<HitsoundEvent>();
		bool clearHitsounds = false;

		List<HitsoundEvent> newPreviewHitsoundEvents = null;
		List<HitsoundEvent> previewHitsoundEvents = new List<HitsoundEvent>();

		QNT_Timestamp hitSoundEnd = new QNT_Timestamp(0);

		void AddHitsoundEvents(List<HitsoundEvent> events, QNT_Timestamp start, QNT_Timestamp end) {
			if(Timeline.orderedNotes.Count == 0) {
				return;
			}

			if(start > end) {
				QNT_Timestamp temp = start;
				start = end;
				end = temp;
			}

			float startTime = timeline.TimestampToSeconds(start);

			var result = timeline.FindFirstNoteAtTime(start);
			for(int i = result; i != -1 && i < Timeline.orderedNotes.Count; ++i) {
				TargetData data = Timeline.orderedNotes[i].data;
				if(data.time < start) {
					continue;
				}
				if(data.time > end) {
					return;
				}

				bool added = false;
				foreach(HitsoundEvent ev in events) {
					if(ev.ID == data.ID) {
						added = true;
						break;
					}
				}

				if(!added) {
					HitsoundEvent ev;
					ev.ID = data.ID;
					ev.currentSample = 0;
					ev.waitSamples = (UInt64)((timeline.TimestampToSeconds(data.time) - startTime) * sampleRate) << ClipData.PrecisionShift;
					ev.sound = kick;
					ev.time = data.time;
					ev.pan = (data.x / 7.15f);
					ev.volume = 1.0f;

					switch (data.velocity) {
						case TargetVelocity.Standard:
							ev.sound = kick;
							break;

						case TargetVelocity.Percussion:
							ev.sound = percussion;
							break;

						case TargetVelocity.Snare:
							ev.sound = snare;
							break;

						case TargetVelocity.Chain:
							ev.sound = chainNote;
							break;

						case TargetVelocity.ChainStart:
							ev.sound = chainStart;
							break;

						case TargetVelocity.Melee:
							ev.sound = melee;
							break;
					}

					//Only one hitsound at a time per point in time
					for (int j = events.Count - 1; j >= 0; j--) {
						if(events[j].sound == ev.sound) {
							if(events[j].waitSamples == ev.waitSamples) {
								events.RemoveAt(j);
							}
						}
					}

					events.Add(ev);
				}
			}
		}

		void PlayHitsounds(CopyContext ctx, List<HitsoundEvent> events) {
			kick.duckVolume = 0.0f;
			snare.duckVolume = 0.0f;
			percussion.duckVolume = 0.0f;
			chainStart.duckVolume = 0.0f;
			chainNote.duckVolume = 0.0f;
			melee.duckVolume = 0.0f;

			UInt64 waitSpeed = (UInt64)1 << ClipData.PrecisionShift;
			if(ctx.playbackSpeed != 1.0f) {
				waitSpeed = (UInt64)(waitSpeed * ctx.playbackSpeed);
			}

			for (int i = events.Count - 1; i >= 0; i--) {
				HitsoundEvent ev = events[i];
				bool valid = true;

				if(ev.waitSamples > 0) {
					if(waitSpeed > ev.waitSamples) {
						ev.waitSamples = 0;
					}
					else {
						ev.waitSamples -= waitSpeed;
					}
				}
				else {
					ctx.volume = hitSoundVolume * ev.volume;
					ev.sound.currentSample = ev.currentSample;
					ev.sound.pan = ev.pan;
					ev.sound.CopySampleIntoBuffer(ctx);

					ev.sound.duckVolume = Mathf.Clamp(ev.sound.duckVolume + 0.25f, 0.0f, 1.0f);

					if(ev.sound.scaledCurrentSample > ev.sound.samples.Length) {
						events.RemoveAt(i);
						valid = false;
					}
					else {
						ev.currentSample = ev.sound.currentSample;
					}
				}

				if(valid) {
					events[i] = ev;
				}
			}
		}

		void OnAudioFilterRead(float[] bufferData, int bufferChannels) {
			CopyContext ctx;
			ctx.bufferData = bufferData;
			ctx.bufferChannels = bufferChannels;
			ctx.bufferFreq = sampleRate;
			ctx.playbackSpeed = speed;
			ctx.outputValue = 0.0f;

			if(clearHitsounds) {
				hitsoundEvents.Clear();
				hitSoundEnd = new QNT_Timestamp(0);
				clearHitsounds = false;
			}

			if(newPreviewHitsoundEvents != null) {
				List<HitsoundEvent> newPreview = newPreviewHitsoundEvents;
				newPreviewHitsoundEvents = null;

				previewHitsoundEvents.Clear();
				previewHitsoundEvents = newPreview;
			}

			if(playPreview) {
				int dataIndex = 0;

				while(dataIndex < bufferData.Length / bufferChannels && (preview.currentSample < currentPreviewSongSampleEnd) && (preview.scaledCurrentSample) < song.samples.Length) {
					ctx.index = dataIndex;
					ctx.volume = volume;
					ctx.playbackSpeed = speed;
					preview.CopySampleIntoBuffer(ctx);

					ctx.playbackSpeed = 1.0f;
					PlayHitsounds(ctx, previewHitsoundEvents);

					++dataIndex;
				}

				if(preview.currentSample >= currentPreviewSongSampleEnd || (preview.scaledCurrentSample) >= song.samples.Length) {
					playPreview = false;
				}
			}

			if (paused || (song.scaledCurrentSample) > song.samples.Length) {
				hitsoundEvents.Clear();

				//Continue to flush the hitsounds
				if(!playPreview && previewHitsoundEvents.Count > 0) {
					int dataIndex = 0;

					while(dataIndex < bufferData.Length / bufferChannels) {
						ctx.index = dataIndex;
						ctx.volume = hitSoundVolume;
						ctx.playbackSpeed = 1.0f;
						PlayHitsounds(ctx, previewHitsoundEvents);
						++dataIndex;
					}
				}

				return;
			}

			if(song != null) {
				QNT_Timestamp timeStart = timeline.ShiftTick(new QNT_Timestamp(0), (float)song.currentTime);
				QNT_Timestamp timeEnd = timeline.ShiftTick(new QNT_Timestamp(0), (float)(song.currentTime + (bufferData.Length / bufferChannels) / (float)song.frequency * (song.frequency / (float)sampleRate)));

				if(timeEnd > hitSoundEnd) {
					hitSoundEnd = timeStart + Constants.QuarterNoteDuration;
					AddHitsoundEvents(hitsoundEvents, timeStart, hitSoundEnd);
				}
			}

			for (int dataIndex = 0; dataIndex < bufferData.Length / bufferChannels; dataIndex++) {
				ctx.volume = volume;
				ctx.index = dataIndex;
				ctx.playbackSpeed = speed;
				song.CopySampleIntoBuffer(ctx);

				//Play hitsounds (reverse iterate so we can remove)
				ctx.volume = hitSoundVolume;
				PlayHitsounds(ctx, hitsoundEvents);

				ctx.playbackSpeed = speed;
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

				//Metronome
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