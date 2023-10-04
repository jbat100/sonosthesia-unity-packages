using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    // MessagePackObject must be public
    
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
    
    internal static class PackedMIDINoteExtensions
    {
        public static MIDINote Unpack(this PackedMIDINote note)
        {
            return new MIDINote(note.Channel, note.Note, note.Velocity);
        }

        public static PackedMIDINote Pack(this MIDINote note, string port)
        {
            return new PackedMIDINote()
            {
                Port = port,
                Channel = note.Channel,
                Note = note.Note,
                Velocity = note.Velocity
            };
        }
    }
}