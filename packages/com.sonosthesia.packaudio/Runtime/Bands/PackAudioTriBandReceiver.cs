using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio
{
    public class PackAudioTriBandReceiver : PackAudioBandReceiver<PackedAudioTriBands>
    {
        protected override string PackAddress => PackAudioBandAddress.BANDS_3;
    }
}