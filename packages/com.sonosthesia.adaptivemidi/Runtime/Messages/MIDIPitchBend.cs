using System;

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
            Channel = channel;
            Value = value;
        }
        
        public MIDIPitchBend(int channel, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = channel;
            Value = value;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIPitchBend)} <{nameof(Channel)} {Channel} {nameof(Value)} {Value} >";
        }
    }
}