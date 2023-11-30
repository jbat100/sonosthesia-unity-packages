using MessagePack;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    internal static class PackMIDIAddress
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
    
    public interface IPackedAddressedMIDIMessage 
    {
        string Port { get; }
        
        string Track { get; }
    }
    
    
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

    [MessagePackObject]
    public class PackedMIDIClock
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("count")]
        public int Count { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedMIDIClock)} {Port} {Count}";
        }
    }

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
    
    [MessagePackObject]
    public class PackedMIDINote : IPackedAddressedMIDIMessage
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
            return $"{nameof(PackedMIDINote)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}";
        }
    }

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

    [MessagePackObject]
    public class PackedMIDIPolyphonicAftertouch : IPackedAddressedMIDIMessage
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
            return $"{nameof(PackedMIDINote)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}";
        }
    }

    public static class PackedMIDIExtensions
    {
        public static bool Check(this IPackedAddressedMIDIMessage message, string port, string track)
        {
            return (string.IsNullOrEmpty(port) || message.Port == port) &&
                   (string.IsNullOrEmpty(track) || message.Track == track);
        }
        
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
        
        public static MIDIClock Unpack(this PackedMIDIClock clock)
        {
            return new MIDIClock(clock.Count);
        }

        public static PackedMIDIClock Pack(this MIDIClock clock, string port)
        {
            return new PackedMIDIClock()
            {
                Count = clock.Count
            };
        }
        
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