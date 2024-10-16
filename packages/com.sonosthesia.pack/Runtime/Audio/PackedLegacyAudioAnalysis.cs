using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject()]
    public class PackedLegacyAudioAnalysis
    {
        [Key("time")]
        public float Time { get; set; }
        
        [Key("rms")]
        public float RMS { get; set; }
        
        [Key("lows")]
        public float Lows { get; set; }
        
        [Key("mids")]
        public float Mids { get; set; }
        
        [Key("highs")]
        public float Highs { get; set; }
        
        [Key("centroid")]
        public float Centroid { get; set; }
        
        [Key("offset")]
        public bool Offset { get; set; }
    }
    
    // forces code generation for audio analysis array
    [MessagePackObject()]
    public class PackedLegacyAudioAnalysisArray
    {
        [Key("items")]
        public PackedLegacyAudioAnalysis[] Items { get; set; }
    }
}