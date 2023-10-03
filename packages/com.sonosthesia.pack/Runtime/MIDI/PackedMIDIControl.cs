using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIControl
    {
        [Key("port")]
        public string Port { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("number")]
        public int Number { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
    }

    public static class PackedMIDIControlExtensions
    {
        public static MIDIControl Unpack(this PackedMIDIControl control)
        {
            return new MIDIControl(control.Channel, control.Number, control.Value);
        }
    }
}