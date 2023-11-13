namespace Sonosthesia.Pack
{
    public class PackAudioTriBandReceiver : PackAudioBandReceiver<PackedAudioTriBands>
    {
        protected override string PackAddress => PackAudioAddress.BANDS_3;
    }
}