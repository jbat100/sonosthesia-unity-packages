using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIRawSourceSingle : IPackedMIDIRawSourceData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("deltaTime")]
        public double DeltaTime { get; set; }
        
        [Key("cumulativeTime")]
        public double CumulativeTime { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
    }
}