namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDISongPositionPointer
    {
        // http://midi.teragonaudio.com/tech/midispec/ssp.htm
        // Position is the number of MIDI beats (16th notes)
        
        public readonly int Position; 

        public MIDISongPositionPointer(int position)
        {
            Position = position;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDISongPositionPointer)} <{nameof(Position)} {Position}>";
        }
    }
}