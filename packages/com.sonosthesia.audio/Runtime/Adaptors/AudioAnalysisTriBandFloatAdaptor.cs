using Sonosthesia.Flow;
using Sonosthesia.Signal;

namespace Sonosthesia.Audio
{
    public class AudioAnalysisTriBandFloatAdaptor : MapAdaptor<AudioAnalysis, TriBand<float>>
    {
        protected override TriBand<float> Map(AudioAnalysis source)
        {
            return new TriBand<float>(source.lows, source.mids, source.highs);
        }
    }
}