using UnityEngine;

namespace Sonosthesia.Processing
{
    [CreateAssetMenu(fileName = "FloatAudioLevel", menuName = "Sonosthesia/Processing/FloatAudioLevel")]
    public class FloatAudioLevelProcessorFactory : DynamicProcessorFactory<float>
    {
        [SerializeField] private FloatWarpSettings _warp;

        [SerializeField] private FloatOneEuroFilterSettings _oneEuroFilter;

        [SerializeField] private FloatSoftLandingSettings _softLanding;
        
        public override IDynamicProcessor<float> Make()
        {
            return new ProcessorChain<float>(
                new FloatWarpProcessor(_warp),
                new FloatOneEuroFilterProcessor(_oneEuroFilter),
                new FloatSoftLandingProcessor(_softLanding)
                );
        }
    }
}