using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedMIDIRawSourceDouble : IPackedMIDIRawSourceData
    {
        [Key("port")]
        public string Port { get; set; }
        
        [Key("deltaTime")]
        public double DeltaTime { get; set; }
        
        [Key("cumulativeTime")]
        public double CumulativeTime { get; set; }
        
        [Key("b0")]
        public byte B0 { get; set; }
        
        [Key("b1")]
        public byte B1 { get; set; }
    }
}