using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    // MessagePackObject must be public
    
    [MessagePackObject]
    public class PackedMIDIPolyphonicAftertouch : IPackedMIDIPortMessage
    {
        [Key("port")]
        public string Port { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("note")]
        public int Note { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
    }
    
    internal static class PackedMIDIPolyphonicAftertouchExtensions
    {
        public static MIDIPolyphonicAftertouch Unpack(this PackedMIDIPolyphonicAftertouch note)
        {
            return new MIDIPolyphonicAftertouch(note.Channel, note.Note, note.Value);
        }

        public static PackedMIDIPolyphonicAftertouch Pack(this MIDIPolyphonicAftertouch aftertouch, string port)
        {
            return new PackedMIDIPolyphonicAftertouch()
            {
                Port = port,
                Channel = aftertouch.Channel,
                Note = aftertouch.Note,
                Value = aftertouch.Value
            };
        }
    }
}