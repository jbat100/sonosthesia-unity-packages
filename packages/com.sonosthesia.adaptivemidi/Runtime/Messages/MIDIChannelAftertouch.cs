namespace Sonosthesia.AdaptiveMIDI.Messages
{
    // polyphonic aftertouch and channel aftertouch are different kinds of messages 
    // https://learn.sparkfun.com/tutorials/midi-tutorial/advanced-messages
    
    public readonly struct MIDIChannelAftertouch
    {
        public readonly int Channel;
        public readonly int Value;
        
        public MIDIChannelAftertouch(int channel, int value)
        {
            Channel = channel;
            Value = value;
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIChannelAftertouch)} <{nameof(Channel)} {Channel} {nameof(Channel)} {Channel}>";
        }
    }
}