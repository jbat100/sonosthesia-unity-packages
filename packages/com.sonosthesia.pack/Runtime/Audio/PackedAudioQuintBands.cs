using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedAudioQuintBands
    {
        [Key("track")]
        public string Track { get; set; }

        [Key("b1")]
        public float B1 { get; set; }
        
        [Key("b2")]
        public float B2 { get; set; }
        
        [Key("b3")]
        public float B3 { get; set; }
        
        [Key("b4")]
        public float B4 { get; set; }
        
        [Key("b5")]
        public float B5 { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(PackedAudioQuintBands)} {B1} {B2} {B3} {B4} {B5}";
        }
    }

    public static class PackedAudioQuintBandsExtensions
    {
        public static QuintAudioBands Unpack(this PackedAudioQuintBands bands)
        {
            return new QuintAudioBands(bands.B1, bands.B2, bands.B3, bands.B4, bands.B5);
        }
        
        public static float GetBand(this PackedAudioQuintBands bands, int index)
        {
            return index switch
            {
                1 => bands.B1,
                2 => bands.B2,
                3 => bands.B3,
                4 => bands.B4,
                5 => bands.B5,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }
}