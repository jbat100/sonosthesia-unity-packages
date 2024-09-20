using MessagePack;

namespace Sonosthesia.Pack
{
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
}