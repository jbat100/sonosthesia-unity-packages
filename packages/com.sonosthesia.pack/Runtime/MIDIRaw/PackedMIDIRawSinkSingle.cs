using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIRawSinkSingle : IPackedMIDIRawSinkData
    {
        [Key("port")]
        public string Port { get; }
        
        [Key("b0")]
        public byte B0 { get; set; }
    }
}