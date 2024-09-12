using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject()]
    public class PackedAudioAnalysis
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
    
    public static class PackedAudioAnalysisExtensions
    {
        public static AudioAnalysis Unpack(this PackedAudioAnalysis analysis)
        {
            return new AudioAnalysis
            {
                time = analysis.Time,
                rms = analysis.RMS,
                lows = analysis.Lows,
                mids = analysis.Mids,
                highs = analysis.Highs,
                centroid = analysis.Centroid,
                offset = analysis.Offset
            };
        }
    }
}