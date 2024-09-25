using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedXAA
    {
        [Key("continuous")]
        public PackedContinuousAudioAnalysis[] Continuous { get; set; }
        
        [Key("peaks")]
        public PackedAudioPeak[] Peaks { get; set; }
        
        [Key("info")]
        public PackedXAAInfo Info { get; set; }
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

    [MessagePackObject]
    public class PackedXAAInfo
    {
        [Key("duration")]
        public float Duration { get; set; }
        
        [Key("main")]
        public PackedAudioSignalInfo Main { get; set; }
        
        [Key("lows")]
        public PackedAudioSignalInfo Lows { get; set; }
        
        [Key("mids")]
        public PackedAudioSignalInfo Mids { get; set; }
        
        [Key("highs")]
        public PackedAudioSignalInfo Highs { get; set; }

        [Key("centroid")]
        public PackedRange Centroid { get; set; }
    }
    
    [MessagePackObject]
    public class PackedAudioSignalInfo
    {
        [Key("band")]
        public PackedRange Band { get; set; }
        
        [Key("magnitude")]
        public PackedRange Magnitude { get; set; }
        
        [Key("peaks")]
        public int Peaks { get; set; }
    }
}