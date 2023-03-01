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
        public readonly MIDISyncType Type;

        public MIDISync(MIDISyncType type)
        {
            Type = type;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDISync)} <{nameof(Type)} {Type}>";
        }
    }
}