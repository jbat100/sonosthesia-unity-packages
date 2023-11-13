using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedAudioTriBands : IPackedAudioBands
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

        public int BandCount => 3;
        
        public float GetBand(int index)
        {
            return index switch
            {
                0 => B1,
                1 => B2,
                2 => B3,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }
    
    public static class PackedAudioTriBandsExtensions
    {
        public static TriAudioBands Unpack(this PackedAudioTriBands bands)
        {
            return new TriAudioBands(bands.B1, bands.B2, bands.B3);
        }
    }
}