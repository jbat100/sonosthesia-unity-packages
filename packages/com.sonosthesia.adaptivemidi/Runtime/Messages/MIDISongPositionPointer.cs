using System;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDISongPositionPointer
    {
        // http://midi.teragonaudio.com/tech/midispec/ssp.htm
        // Position is the number of MIDI beats (16th notes) 7 bit [0-16383]
        
        public readonly TimeSpan Timestamp;
        public readonly int Position; 

        public MIDISongPositionPointer(TimeSpan timestamp, int position)
        {
            Timestamp = timestamp;
            Position = position;
        }
        
        public MIDISongPositionPointer(int position)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Position = position;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDISongPositionPointer)} <{nameof(Position)} {Position}>";
        }
    }
}