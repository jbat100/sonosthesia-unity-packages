using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMPEAftertouch
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
            return $"{nameof(PackedMPEAftertouch)} {nameof(Port)} {Port} {nameof(Track)} {Track} " +
                   $"{nameof(Channel)} {Channel} {nameof(Value)} {Value}";
        }
    }
}