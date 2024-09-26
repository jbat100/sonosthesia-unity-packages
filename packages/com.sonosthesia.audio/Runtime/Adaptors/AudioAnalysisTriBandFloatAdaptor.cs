using Sonosthesia.Flow;
using Sonosthesia.Signal;

namespace Sonosthesia.Audio
{
    public class AudioAnalysisTriBandFloatAdaptor : MapAdaptor<ContinuousAnalysis, TriBand<float>>
    {
        protected override TriBand<float> Map(ContinuousAnalysis source)
        {
            return new TriBand<float>(source.lows, source.mids, source.highs);
        }
    }
}