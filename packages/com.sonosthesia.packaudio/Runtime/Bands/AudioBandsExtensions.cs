using Sonosthesia.Audio;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public static class PackedAudioQuintBandsExtensions
    {
        public static QuintAudioBands Unpack(this PackedAudioQuintBands bands)
        {
            return new QuintAudioBands(bands.B1, bands.B2, bands.B3, bands.B4, bands.B5);
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