using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIRawSinkTripple : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; }
        
        [Key("b0")]
        public byte B0 { get; set; }
        
        [Key("b1")]
        public byte B1 { get; set; }
        
        [Key("b2")]
        public byte B2 { get; set; }
    }
}