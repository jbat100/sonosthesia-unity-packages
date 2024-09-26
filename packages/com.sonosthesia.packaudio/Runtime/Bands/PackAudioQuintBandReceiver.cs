using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public class PackAudioQuintBandReceiver : PackAudioBandReceiver<PackedAudioQuintBands>
    {
        protected override string PackAddress => PackAudioBandAddress.BANDS_5;
    }
}