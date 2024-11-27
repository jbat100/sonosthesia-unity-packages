using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatRangeSettings : DynamicProcessorSettings
    {
        [SerializeField] private float _lower = 0f;
        public float Lower => _lower;

        [SerializeField] private float _upper;
        public float Upper => _upper;

        [SerializeField] private bool _clip;
        public bool Clip => _clip;
    }

    public class FloatRangeProcessor : DynamicProcessor<float, FloatRangeSettings>
    {
        public FloatRangeProcessor(FloatRangeSettings settings) : base(settings) { }

        protected override float Process(FloatRangeSettings settings, float input, float time)
        {
            input = math.remap(0f, 1f, settings.Lower, settings.Upper, input);
            if (settings.Clip)
            {
                math.clamp(input, settings.Lower, settings.Upper);
            }
            return input;
        }
    }
    
    // note : this processor expects a normalized (0-1) input and will remap it to a specified range
    // note : is equivalent to the remap processor but with input range set to (0, 1)
    
    [CreateAssetMenu(fileName = "FloatRange", menuName = "Sonosthesia/Processing/FloatRange")]
    public class FloatRangeProcessorFactory : 
        ConfigurableDynamicProcessorFactory<FloatRangeSettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatRangeSettings settings)
            => new FloatRangeProcessor(settings);
    }
}