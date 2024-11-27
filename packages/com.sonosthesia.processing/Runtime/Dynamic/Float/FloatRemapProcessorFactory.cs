using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatRemapSettings : DynamicProcessorSettings
    {
        [SerializeField] private bool _clamp;
        [SerializeField] private float _fromMin;
        [SerializeField] private float _fromMax = 1;
        [SerializeField] private float _toMin;
        [SerializeField] private float _toMax = 1;

        public float Remap(float input)
        {
            return MathUtils.Remap(input, _fromMin, _fromMax, _toMin, _toMax, _clamp);
        }
    }

    public class FloatRemapProcessor : DynamicProcessor<float, FloatRemapSettings>
    {
        public FloatRemapProcessor(FloatRemapSettings settings) : base(settings) { }

        protected override float Process(FloatRemapSettings settings, float input, float time)
        {
            return settings.Remap(input);
        }
    }
    
    [CreateAssetMenu(fileName = "FloatRemap", menuName = "Sonosthesia/Processing/FloatRemap")]
    public class FloatRemapProcessorFactory 
        : ConfigurableDynamicProcessorFactory<FloatRemapSettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatRemapSettings settings)
            => new FloatRemapProcessor(settings);
    }
}