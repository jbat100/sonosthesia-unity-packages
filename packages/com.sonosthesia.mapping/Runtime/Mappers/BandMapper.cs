using System;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class BandMapper : Mapper<float>
    {
        [Serializable]
        public class ProcessorSettings : DynamicProcessorSettings
        {
            [SerializeField] private FloatWarpSettings _warp;
            [SerializeField] private FloatOneEuroFilterSettings _oneEuroFilter;
            [SerializeField] private FloatSoftLandingSettings _softLanding;

            public IDynamicProcessor<float> Make()
            {
                return new ProcessorChain<float>(
                    new FloatWarpProcessor(_warp),
                    new FloatOneEuroFilterProcessor(_oneEuroFilter),
                    new FloatSoftLandingProcessor(_softLanding)
                );
            }
        }
    }
}