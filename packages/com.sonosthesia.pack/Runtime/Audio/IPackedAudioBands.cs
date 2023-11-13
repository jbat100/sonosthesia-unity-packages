namespace Sonosthesia.Pack
{
    public interface IPackedAudioBands
    {
        string Track { get; }

        int BandCount { get; }
        
        float GetBand(int index);
    }
}