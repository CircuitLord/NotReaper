using System;
using System.Text;
using UnityEngine;

namespace NotReaper.Timing {
    class Constants {
        public static UInt64 PulsesPerQuarterNote = 480;
        public static QNT_Duration QuarterNoteDuration = new QNT_Duration(PulsesPerQuarterNote);
        public static QNT_Duration SixteenthNoteDuration = new QNT_Duration(PulsesPerQuarterNote / 4);

        public static UInt64 OneMinuteInMicroseconds = 60000000;
        public static UInt64 SecondsToMicroseconds = 1000000;

        public static double GetBPMFromMicrosecondsPerQuaterNote(UInt64 microsecondsPerQuarterNote) {
            return (double)OneMinuteInMicroseconds / microsecondsPerQuarterNote;
        }

        public static UInt64 MicrosecondsPerQuarterNoteFromBPM(double bpm) {
            return (UInt64)Math.Round((double)OneMinuteInMicroseconds / bpm);
        }

        public static QNT_Duration DurationFromBeatSnap(uint beatSnap) {
            QNT_Duration duration = Constants.QuarterNoteDuration;

            //Since we use PPQN, anything a quarter note or smaller is a division of the time
			if(beatSnap >= 4) {
				duration /= (beatSnap / 4);
			}
			//Otherwise, it is a multiplication
			else {
				duration.tick = (UInt64)((4.0f / beatSnap) * duration.tick);
			}

            return duration;
        }
    }

    class Conversion {
       public static Relative_QNT ToQNT(float seconds, UInt64 microsecondsPerQuarterNote) {
           return new Relative_QNT((long)Mathf.Round((float)(Constants.SecondsToMicroseconds * seconds) / microsecondsPerQuarterNote * Constants.PulsesPerQuarterNote));
       }

       public static float FromQNT(Relative_QNT duration, UInt64 microsecondsPerQuarterNote) {
        return (duration.tick * (long)microsecondsPerQuarterNote) / ((float)Constants.SecondsToMicroseconds * Constants.PulsesPerQuarterNote);
       }

       public static string MicrosecondsToString(UInt64 microseconds) {
           double bpm = (60.0 * ((double)Constants.SecondsToMicroseconds / microseconds));
           return String.Format(bpm == (UInt64)bpm ? "{0:0}" : "{0:0.00}", bpm);

           //UInt64 seconds = microseconds / Constants.MicrosecondsToSeconds;
           //StringBuilder sb = new StringBuilder();
           //sb.Append(seconds.ToString());
           //
           //UInt64 remainder = microseconds - seconds;
           //if(remainder != 0) {
           //    sb.Append("."); 
           //}
//
           //return sb.ToString();
       }
    };

    /// <summary>
    /// QNT (or quarter note ticks), represents a value in pulses per quarter note
    /// This value represents a point in time in the current song
    /// </summary>
    public struct QNT_Timestamp {
        public QNT_Timestamp(UInt64 tick) {
            this.tick = tick;
        }

        public UInt64 tick;

        public static QNT_Timestamp GetSnappedValue(QNT_Timestamp time, int beatSnap) {
            QNT_Duration snapValue = Constants.DurationFromBeatSnap((uint)beatSnap);
			return (time / snapValue) * snapValue;
        }

        public float ToBeatTime() {
            return tick / (float)Constants.PulsesPerQuarterNote;
        }

        public int CompareTo(QNT_Timestamp other) {
            return tick.CompareTo(other.tick);
        }

        public static bool operator ==(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick == b.tick;
        }

        public static bool operator !=(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick != b.tick;
        }

        public static bool operator <(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick < b.tick;
        }
        public static bool operator >(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick > b.tick;
        }
        public static bool operator <=(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick <= b.tick;
        }
        public static bool operator >=(QNT_Timestamp a, QNT_Timestamp b) {
            return a.tick >= b.tick;
        }

        public static QNT_Timestamp operator +(QNT_Timestamp a, Relative_QNT b) {
            Int64 result = (Int64)a.tick + b.tick;
            if(result < 0) {
                result = 0;
            }

            return new QNT_Timestamp((UInt64)result);
        }

        public static QNT_Timestamp operator -(QNT_Timestamp a, Relative_QNT b) {
            Int64 result = (Int64)a.tick - b.tick;
            if(result < 0) {
                result = 0;
            }

            return new QNT_Timestamp((UInt64)result);
        }

        public static Relative_QNT operator -(QNT_Timestamp a, QNT_Timestamp b) {
            return new Relative_QNT((Int64)a.tick - (Int64)b.tick);
        }

        public static QNT_Timestamp operator +(QNT_Timestamp a, QNT_Duration b) {
            return new QNT_Timestamp(a.tick + b.tick);
        }

        public static QNT_Timestamp operator -(QNT_Timestamp a, QNT_Duration b) {
            if(b.tick > a.tick) {
                return new QNT_Timestamp(0);
            }

            return new QNT_Timestamp(a.tick - b.tick);
        }

        public static QNT_Timestamp operator *(QNT_Timestamp a, QNT_Duration b) {
            return new QNT_Timestamp(a.tick * b.tick);
        }

        public static QNT_Timestamp operator /(QNT_Timestamp a, QNT_Duration b) {
            return new QNT_Timestamp(a.tick / b.tick);
        }
    }

    /// <summary>
    /// QNT (or quarter note ticks), represents a value in pulses per quarter note
    /// This value represents a relative offset from a timestamp
    /// </summary>
    public struct Relative_QNT {
        public Relative_QNT(Int64 tick) {
            this.tick = tick;
        }

        public Int64 tick;

        public float ToBeatTime() {
            return tick / (float)Constants.PulsesPerQuarterNote;
        }

        public int CompareTo(Relative_QNT other) {
            return tick.CompareTo(other.tick);
        }

        public static bool operator ==(Relative_QNT a, long b) {
            return a.tick == b;
        }

        public static bool operator !=(Relative_QNT a, long b) {
            return a.tick != b;
        }

        public static bool operator ==(Relative_QNT a, Relative_QNT b) {
            return a.tick == b.tick;
        }

        public static bool operator !=(Relative_QNT a, Relative_QNT b) {
            return a.tick != b.tick;
        }

        public static bool operator <(Relative_QNT a, Relative_QNT b) {
            return a.tick < b.tick;
        }
        public static bool operator >(Relative_QNT a, Relative_QNT b) {
            return a.tick > b.tick;
        }
        public static bool operator <=(Relative_QNT a, Relative_QNT b) {
            return a.tick <= b.tick;
        }
        public static bool operator >=(Relative_QNT a, Relative_QNT b) {
            return a.tick >= b.tick;
        }

        public static Relative_QNT operator +(Relative_QNT a, Relative_QNT b)
            => new Relative_QNT(a.tick + b.tick);

        public static Relative_QNT operator -(Relative_QNT a, Relative_QNT b)
            => new Relative_QNT(a.tick - b.tick);

        public static Relative_QNT operator *(Relative_QNT a, Relative_QNT b)
            => new Relative_QNT(a.tick * b.tick);

        public static Relative_QNT operator /(Relative_QNT a, Relative_QNT b)
            => new Relative_QNT(a.tick / b.tick);
    }

    /// <summary>
    /// QNT (or quarter note ticks), represents a value in pulses per quarter note
    /// This value represents a positive duration of time
    /// </summary>
    public struct QNT_Duration {
        public QNT_Duration(UInt64 tick) {
            this.tick = tick;
        }

        public UInt64 tick;

        public static QNT_Duration GetSnappedValue(QNT_Duration duration, int beatSnap) {
            QNT_Duration snapValue = Constants.DurationFromBeatSnap((uint)beatSnap);
			return (duration / snapValue.tick) * snapValue.tick;
        }

        public static QNT_Duration FromBeatTime(float beatTime) {
            return new QNT_Duration((UInt64)Mathf.Round(beatTime * (float)Constants.PulsesPerQuarterNote));
        }

        public float ToBeatTime() {
            return tick / (float)Constants.PulsesPerQuarterNote;
        }

        public int CompareTo(QNT_Duration other) {
            return tick.CompareTo(other.tick);
        }

        public static bool operator ==(QNT_Duration a, UInt64 b) {
            return a.tick == b;
        }

        public static bool operator !=(QNT_Duration a, UInt64 b) {
            return a.tick != b;
        }

        public static bool operator ==(QNT_Duration a, QNT_Duration b) {
            return a.tick == b.tick;
        }

        public static bool operator !=(QNT_Duration a, QNT_Duration b) {
            return a.tick != b.tick;
        }

        public static bool operator <(QNT_Duration a, QNT_Duration b) {
            return a.tick < b.tick;
        }
        public static bool operator >(QNT_Duration a, QNT_Duration b) {
            return a.tick > b.tick;
        }
        public static bool operator <=(QNT_Duration a, QNT_Duration b) {
            return a.tick <= b.tick;
        }
        public static bool operator >=(QNT_Duration a, QNT_Duration b) {
            return a.tick >= b.tick;
        }

        public static QNT_Duration operator +(QNT_Duration a, QNT_Duration b)
            => new QNT_Duration(a.tick + b.tick);

        public static QNT_Duration operator -(QNT_Duration a, QNT_Duration b)
            => new QNT_Duration(a.tick - b.tick);

        public static QNT_Duration operator *(QNT_Duration a, UInt64 b)
            => new QNT_Duration(a.tick * b);

        public static QNT_Duration operator /(QNT_Duration a, UInt64 b)
            => new QNT_Duration(a.tick / b);
    }
}