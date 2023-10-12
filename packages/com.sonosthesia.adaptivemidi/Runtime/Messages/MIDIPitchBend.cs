namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDIPitchBend
    {
        public readonly int Channel;
        public readonly int Value; // [-8192 8191]
        
        public MIDIPitchBend(int channel, int value)
        {
            Channel = channel;
            Value = value;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIPitchBend)} <{nameof(Channel)} {Channel} {nameof(Value)} {Value} >";
        }
    }
}