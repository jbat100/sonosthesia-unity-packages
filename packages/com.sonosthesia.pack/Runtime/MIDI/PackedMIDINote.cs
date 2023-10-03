using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDINote
    {
        [Key("port")]
        public string Port { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("note")]
        public int Note { get; set; }
        
        [Key("velocity")]
        public int Velocity { get; set; }
    }
    
    public static class PackedMIDINoteExtensions
    {
        public static MIDINote Unpack(this PackedMIDINote note)
        {
            return new MIDINote(note.Channel, note.Note, note.Velocity);
        }
    }
}