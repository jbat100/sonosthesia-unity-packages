using System;

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
            Channel = channel;
            Number = number;
            Value = value;
        }
        
        public MIDIControl(TimeSpan timestamp, int channel, int number, int value)
        {
            Timestamp = timestamp;
            Channel = channel;
            Number = number;
            Value = value;
        }

        public override string ToString()
        {
            return $"{nameof(MIDIControl)} <{nameof(Channel)} {Channel} {nameof(Number)} {Number} {nameof(Value)} {Value} {nameof(Timestamp)} {Timestamp}>";
        }
    }
}