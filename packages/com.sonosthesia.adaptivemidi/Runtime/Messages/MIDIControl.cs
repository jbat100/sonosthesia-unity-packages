using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDIControl
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Number;
        public readonly int Value;

        public MIDIControl(int channel, int number, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Number = Mathf.Clamp(number, 0, 127);
            Value = Mathf.Clamp(value, 0, 127);
        }
        
        public MIDIControl(TimeSpan timestamp, int channel, int number, int value)
        {
            Timestamp = timestamp;
            Channel = Mathf.Clamp(channel, 0, 15);
            Number = Mathf.Clamp(number, 0, 127);;
            Value = Mathf.Clamp(value, 0, 127);
        }

        public override string ToString()
        {
            return $"{nameof(MIDIControl)} <{nameof(Channel)} {Channel} {nameof(Number)} {Number} {nameof(Value)} {Value} {nameof(Timestamp)} {Timestamp.TotalSeconds}>";
        }
    }
}