using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedAudioTriBands
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("b1")]
        public float B1 { get; set; }
        
        [Key("b2")]
        public float B2 { get; set; }
        
        [Key("b3")]
        public float B3 { get; set; }

        public override string ToString()
        {
            return $"{nameof(PackedAudioTriBands)} {Track} {B1} {B2} {B3}";
        }
    }
    
    public static class PackedAudioTriBandsExtensions
    {
        public static TriAudioBands Unpack(this PackedAudioTriBands bands)
        {
            return new TriAudioBands(bands.B1, bands.B2, bands.B3);
        }

        public static float GetBand(this PackedAudioTriBands bands, int index)
        {
            return index switch
            {
                1 => bands.B1,
                2 => bands.B2,
                3 => bands.B3,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }
}