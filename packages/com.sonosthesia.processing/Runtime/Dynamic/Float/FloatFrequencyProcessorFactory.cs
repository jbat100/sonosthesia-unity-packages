using System;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatFrequencySettings : DynamicProcessorSettings
    {
        public enum ConversionType
        {
            RangeToFrequency,
            FrequencyToRange
        }

        private static LogRangeConverter _converter = new (20, 500, 20000);

        [SerializeField] private ConversionType _conversionType = ConversionType.RangeToFrequency;
        [SerializeField] private float _lower = 0;
        [SerializeField] private float _upper = 1;
        [SerializeField] private bool _clamp = true;

        public float Process(float input)
        {
            switch (_conversionType)
            {
                case ConversionType.RangeToFrequency:
                {
                    float normalized = MathUtils.Remap(input, _lower, _upper, 0, 1, _clamp);
                    return _converter.ToRange(normalized);
                }
                case ConversionType.FrequencyToRange:
                {
                    float normalized = _converter.ToNormalized(input);
                    return MathUtils.Remap(normalized, 0, 1, _lower, _upper, _clamp);
                }
                default:
                    return 0;
            }
        }
    }

    public class FloatFrequencyProcessor : DynamicProcessor<float, FloatFrequencySettings>
    {
        public FloatFrequencyProcessor(FloatFrequencySettings settings) : base(settings) { }

        protected override float Process(FloatFrequencySettings settings, float input, float time)
        {
            return settings.Process(input);
        }
    }
    
    [CreateAssetMenu(fileName = "FloatFrequency", menuName = "Sonosthesia/Processing/FloatFrequency")]
    public class FloatFrequencyProcessorFactory : ConfigurableDynamicProcessorFactory<FloatFrequencySettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatFrequencySettings settings) => 
            new FloatFrequencyProcessor(settings);
    }
}