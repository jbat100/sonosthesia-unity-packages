using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDIPitchBend
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Value; // [-8192 8191]
        
        public MIDIPitchBend(TimeSpan timestamp, int channel, int value)
        {
            Timestamp = timestamp;
            Channel = Mathf.Clamp(channel, 0, 15);
            Value = Mathf.Clamp(value, -8192, 8191);
        }
        
        public MIDIPitchBend(int channel, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Value = Mathf.Clamp(value, -8192, 8191);
        }

        public MIDIPitchBend(int channel, float semitones, float range)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Value = Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(semitones, range) * 8191 / range), -8192, 8191);
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIPitchBend)} <{nameof(Channel)} {Channel} {nameof(Value)} {Value} >";
        }
    }
}