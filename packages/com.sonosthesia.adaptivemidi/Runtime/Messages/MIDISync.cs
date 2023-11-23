using System;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public enum MIDISyncType
    {
        Start,
        Stop,
        Continue
    }
    
    public readonly struct MIDISync
    {
        public readonly TimeSpan Timestamp;
        public readonly MIDISyncType Type;

        public MIDISync(TimeSpan timestamp, MIDISyncType type)
        {
            Timestamp = timestamp;
            Type = type;
        }
        
        public MIDISync(MIDISyncType type)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Type = type;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDISync)} <{nameof(Type)} {Type}>";
        }
    }
}