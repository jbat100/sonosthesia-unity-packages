using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIRawSinkDouble : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; }
        
        [Key("b0")]
        public int B0 { get; set; }
        
        [Key("b1")]
        public int B1 { get; set; }
    }
}