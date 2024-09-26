using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatFrequencySettings : DynamicProcessorSettings
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private float _lower;
        public float Lower => _lower;
        
        [SerializeField] private float _upper;
        public float Upper => _upper;

        [SerializeField] private bool _clip;
        public bool Clip => _clip;
    }

    public class FloatFrequencyProcessor : DynamicProcessor<float, FloatFrequencySettings>
    {
        public FloatFrequencyProcessor(FloatFrequencySettings settings) : base(settings) { }

        private const float LOG10_20 = 1.3010299956639813f;
        private const float LOG10_20000 = 4.301029995663981f;
        
        protected override float Process(FloatFrequencySettings settings, float input, float time)
        {
            bool log = settings.Log;
            
            float lower = log ? LOG10_20 : 20f;
            float upper = log ? LOG10_20000 : 20000f;

            if (log)
            {
                input = math.log10(input);
            }
            
            input = math.remap(lower, upper, settings.Lower, settings.Upper, input);
            if (settings.Clip)
            {
                math.clamp(input, settings.Lower, settings.Upper);
            }
            
            return input;
        }
    }
    
    [CreateAssetMenu(fileName = "FloatFrequency", menuName = "Sonosthesia/Processing/FloatFrequency")]
    public class FloatFrequencyProcessorFactory : ConfigurableDynamicProcessorFactory<FloatFrequencySettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatFrequencySettings settings) => 
            new FloatFrequencyProcessor(settings);
    }
}