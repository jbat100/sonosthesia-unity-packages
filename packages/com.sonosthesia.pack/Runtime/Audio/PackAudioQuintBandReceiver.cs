namespace Sonosthesia.Pack
{
    public class PackAudioQuintBandReceiver : PackAudioBandReceiver<PackedAudioQuintBands>
    {
        protected override string PackAddress => PackAudioAddress.BANDS_5;
    }
}