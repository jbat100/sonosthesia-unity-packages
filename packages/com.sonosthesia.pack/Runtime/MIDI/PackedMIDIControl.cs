using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    // MessagePackObject must be public
    
    [MessagePackObject]
    public class PackedMIDIControl : IPackedAddressedMIDIMessage
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("number")]
        public int Number { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedMIDIControl)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Number)} {Number} {nameof(Value)} {Value}";
        }
    }

    internal static class PackedMIDIControlExtensions
    {
        public static MIDIControl Unpack(this PackedMIDIControl control)
        {
            return new MIDIControl(control.Channel, control.Number, control.Value);
        }
        
        public static PackedMIDIControl Pack(this MIDIControl control, string port)
        {
            return new PackedMIDIControl()
            {
                Port = port,
                Channel = control.Channel,
                Number = control.Number,
                Value = control.Value
            };
        }
    }
}