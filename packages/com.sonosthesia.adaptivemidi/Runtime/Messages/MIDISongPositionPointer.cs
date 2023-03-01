namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDISongPositionPointer
    {
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