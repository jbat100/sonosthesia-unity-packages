using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIPitchBend : IPackedAddressedMIDIMessage
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
            return $"{nameof(PackedMIDIPitchBend)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }
    
    internal static class PackedMIDIPitchBendExtensions
    {
        public static MIDIPitchBend Unpack(this PackedMIDIPitchBend note)
        {
            return new MIDIPitchBend(note.Channel, note.Value);
        }

        public static PackedMIDIPitchBend Pack(this MIDIPitchBend aftertouch, string port)
        {
            return new PackedMIDIPitchBend
            {
                Port = port,
                Channel = aftertouch.Channel,
                Value = aftertouch.Value
            };
        }
    }
}