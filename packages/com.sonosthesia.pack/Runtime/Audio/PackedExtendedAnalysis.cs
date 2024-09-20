using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedExtendedAnalysis
    {
        [Key("continuous")]
        public PackedContinuousAudioAnalysis[] Continuous { get; set; }
        
        [Key("peaks")]
        public PackedAudioPeak[] Peaks { get; set; }
    }

    [MessagePackObject]
    public class PackedContinuousAudioAnalysis
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
    }
    
    [MessagePackObject]
    public class PackedAudioPeak
    {
        [Key("channel")]
        public int Channel { get; set; }
        
        [Key("start")]
        public float Start { get; set; }
        
        [Key("duration")]
        public float Duration { get; set; }
        
        [Key("magnitude")]
        public float Magnitude { get; set; }
        
        [Key("strength")]
        public float Strength { get; set; }
    }
}