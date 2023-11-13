using System;
using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class PackedAudioQuintBands : IPackedAudioBands
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

        public int BandCount => 5;
        
        public float GetBand(int index)
        {
            return index switch
            {
                0 => B1,
                1 => B2,
                2 => B3,
                3 => B4,
                4 => B5,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }

    public static class PackedAudioQuintBandsExtensions
    {
        public static QuintAudioBands Unpack(this PackedAudioQuintBands bands)
        {
            return new QuintAudioBands(bands.B1, bands.B2, bands.B3, bands.B4, bands.B5);
        }
    }
}