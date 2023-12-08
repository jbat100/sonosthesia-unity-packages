using System;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDIClock
    {
        public readonly TimeSpan Timestamp;
        public readonly int Count;

        public MIDIClock(int count)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Count = count;
        }
        
        public MIDIClock(TimeSpan timestamp, int count)
        {
            Timestamp = timestamp;
            Count = count;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIClock)} <{nameof(Count)} {Count}, {nameof(Timestamp)} {Timestamp.TotalSeconds}>";
        }
    }
}