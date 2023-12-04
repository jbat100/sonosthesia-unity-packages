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
        string Port { get; }
        
        string Track { get; }
    }
    
    
    [MessagePackObject]
    public class PackedLiveMIDIChannelAftertouch : IPackedLiveMIDIMessage
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
            return $"{nameof(PackedLiveMIDIChannelAftertouch)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIClock
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("count")]
        public int Count { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedLiveMIDIClock)} {Port} {Count}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIControl : IPackedLiveMIDIMessage
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
            return $"{nameof(PackedLiveMIDIControl)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Number)} {Number} {nameof(Value)} {Value}";
        }
    }
    
    [MessagePackObject]
    public class PackedLiveMIDINote : IPackedLiveMIDIMessage
    {
        [Key("port")]
        public string Port { get; set; }
        
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
            return $"{nameof(PackedLiveMIDINote)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIPitchBend : IPackedLiveMIDIMessage
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
            return $"{nameof(PackedLiveMIDIPitchBend)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }

    [MessagePackObject]
    public class PackedLiveMIDIPolyphonicAftertouch : IPackedLiveMIDIMessage
    {
        [Key("port")]
        public string Port { get; set; }
        
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
            return $"{nameof(PackedLiveMIDINote)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}";
        }
    }

    public static class PackedLiveMIDIExtensions
    {
        public static bool Check(this IPackedLiveMIDIMessage message, string port, string track)
        {
            return (string.IsNullOrEmpty(port) || message.Port == port) &&
                   (string.IsNullOrEmpty(track) || message.Track == track);
        }
        
        public static MIDIPolyphonicAftertouch Unpack(this PackedLiveMIDIPolyphonicAftertouch note)
        {
            return new MIDIPolyphonicAftertouch(note.Channel, note.Note, note.Value);
        }

        public static PackedLiveMIDIPolyphonicAftertouch Pack(this MIDIPolyphonicAftertouch aftertouch, string port)
        {
            return new PackedLiveMIDIPolyphonicAftertouch()
            {
                Port = port,
                Channel = aftertouch.Channel,
                Note = aftertouch.Note,
                Value = aftertouch.Value
            };
        }
        
        public static MIDIPitchBend Unpack(this PackedLiveMIDIPitchBend note)
        {
            return new MIDIPitchBend(note.Channel, note.Value);
        }

        public static PackedLiveMIDIPitchBend Pack(this MIDIPitchBend aftertouch, string port)
        {
            return new PackedLiveMIDIPitchBend
            {
                Port = port,
                Channel = aftertouch.Channel,
                Value = aftertouch.Value
            };
        }
        
        public static MIDINote Unpack(this PackedLiveMIDINote note)
        {
            return new MIDINote(note.Channel, note.Note, note.Velocity);
        }

        public static PackedLiveMIDINote Pack(this MIDINote note, string port)
        {
            return new PackedLiveMIDINote()
            {
                Port = port,
                Channel = note.Channel,
                Note = note.Note,
                Velocity = note.Velocity
            };
        }
        
        public static MIDIControl Unpack(this PackedLiveMIDIControl control)
        {
            return new MIDIControl(control.Channel, control.Number, control.Value);
        }
        
        public static PackedLiveMIDIControl Pack(this MIDIControl control, string port)
        {
            return new PackedLiveMIDIControl()
            {
                Port = port,
                Channel = control.Channel,
                Number = control.Number,
                Value = control.Value
            };
        }
        
        public static MIDIClock Unpack(this PackedLiveMIDIClock clock)
        {
            return new MIDIClock(clock.Count);
        }

        public static PackedLiveMIDIClock Pack(this MIDIClock clock, string port)
        {
            return new PackedLiveMIDIClock()
            {
                Count = clock.Count
            };
        }
        
        public static MIDIChannelAftertouch Unpack(this PackedLiveMIDIChannelAftertouch note)
        {
            return new MIDIChannelAftertouch(note.Channel, note.Value);
        }

        public static PackedLiveMIDIChannelAftertouch Pack(this MIDIChannelAftertouch aftertouch, string port)
        {
            return new PackedLiveMIDIChannelAftertouch()
            {
                Port = port,
                Channel = aftertouch.Channel,
                Value = aftertouch.Value
            };
        }
    }
}