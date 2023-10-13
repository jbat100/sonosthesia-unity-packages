using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIChannelAftertouch : IPackedAddressedMIDIMessage
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedMIDIChannelAftertouch)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }
    
    internal static class PackedMIDIChannelAftertouchExtensions
    {
        public static MIDIChannelAftertouch Unpack(this PackedMIDIChannelAftertouch note)
        {
            return new MIDIChannelAftertouch(note.Channel, note.Value);
        }

        public static PackedMIDIChannelAftertouch Pack(this MIDIChannelAftertouch aftertouch, string port)
        {
            return new PackedMIDIChannelAftertouch()
            {
                Port = port,
                Channel = aftertouch.Channel,
                Value = aftertouch.Value
            };
        }
    }
}