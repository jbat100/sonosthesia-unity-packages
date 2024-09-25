using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Audio
{
    [CreateAssetMenu(fileName = "AnalysisMagnitude", menuName = "Sonosthesia/Processing/AnalysisMagnitude")]
    public class AnalysisMagnitudeProcessorFactory : DynamicProcessorFactory<float>
    {
        [SerializeField] private FloatOneEuroFilterSettings _oneEuroFilter;

        [SerializeField] private FloatSoftLandingSettings _softLanding;

        [SerializeField] private FloatRangeSettings _range;
        
        public override IDynamicProcessor<float> Make()
        {
            return new ProcessorChain<float>(
                new FloatOneEuroFilterProcessor(_oneEuroFilter),
                new FloatSoftLandingProcessor(_softLanding),
                new FloatRangeProcessor(_range)
            );
        }
    }
}