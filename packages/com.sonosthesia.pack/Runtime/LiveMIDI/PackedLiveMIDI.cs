using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    internal static class PackLiveMIDIAddress
    {
        public const string NOTE                    = "/midi/note";
        public const string NOTE_ON                 = "/midi/note/on";
        public const string NOTE_OFF                = "/midi/note/off";
        public const string POLYPHONIC_AFTERTOUCH   = "/midi/note/aftertouch";
        public const string CONTROL                 = "/midi/channel/control";
        public const string CHANNEL_AFTERTOUCH      = "/midi/channel/aftertouch";
        public const string PITCH_BEND              = "/midi/channel/bend";
        public const string CLOCK                   = "/midi/clock";
    }
    
    public interface IPackedLiveMIDIMessage 
    {
        string Track { get; }
    }
    
    [MessagePackObject]
    public class PackedLiveMIDIChannelAftertouch : IPackedLiveMIDIMessage
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedLiveMIDIChannelAftertouch)} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIControl : IPackedLiveMIDIMessage
    {
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
            return $"{nameof(PackedLiveMIDIControl)} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Number)} {Number} {nameof(Value)} {Value}";
        }
    }
    
    [MessagePackObject]
    public class PackedLiveMIDINote : IPackedLiveMIDIMessage
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("note")]
        public int Note { get; set; }
        
        [Key("velocity")]
        public int Velocity { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedLiveMIDINote)} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIPitchBend : IPackedLiveMIDIMessage
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedLiveMIDIPitchBend)} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIPolyphonicAftertouch : IPackedLiveMIDIMessage
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("note")]
        public int Note { get; set; }
        
        [Key("value")]
        public int Value { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedLiveMIDINote)} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}";
        }
    }

    public static class PackedLiveMIDIExtensions
    {
        public static bool Check(this IPackedLiveMIDIMessage message, string track)
        {
            return string.IsNullOrEmpty(track) || message.Track == track;
        }
        
        public static MIDIPolyphonicAftertouch Unpack(this PackedLiveMIDIPolyphonicAftertouch note)
        {
            return new MIDIPolyphonicAftertouch(note.Channel, note.Note, note.Value);
        }

        public static MIDIPitchBend Unpack(this PackedLiveMIDIPitchBend note)
        {
            return new MIDIPitchBend(note.Channel, note.Value);
        }

        public static MIDINoteOn UnpackNoteOn(this PackedLiveMIDINote note)
        {
            return new MIDINoteOn(note.Channel, note.Note, note.Velocity);
        }
        
        public static MIDINoteOff UnpackNoteOff(this PackedLiveMIDINote note)
        {
            return new MIDINoteOff(note.Channel, note.Note, note.Velocity);
        }

        public static MIDIControl Unpack(this PackedLiveMIDIControl control)
        {
            return new MIDIControl(control.Channel, control.Number, control.Value);
        }

        public static MIDIChannelAftertouch Unpack(this PackedLiveMIDIChannelAftertouch note)
        {
            return new MIDIChannelAftertouch(note.Channel, note.Value);
        }
    }
}