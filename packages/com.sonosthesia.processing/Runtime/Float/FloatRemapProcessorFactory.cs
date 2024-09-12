using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatRemapSettings : DynamicProcessorSettings
    {
        [SerializeField] private bool _clamp;
        public bool Clamp => _clamp;
        
        [SerializeField] private float _fromMin;
        public float FromMin => _fromMin;

        [SerializeField] private float _fromMax = 1;
        public float FromMax => _fromMax;
        
        [SerializeField] private float _toMin;
        public float ToMin => _toMin;

        [SerializeField] private float _toMax = 1;
        public float ToMax => _toMax;
    }

    public class FloatRemapProcessor : DynamicProcessor<float, FloatRemapSettings>
    {
        public FloatRemapProcessor(FloatRemapSettings settings) : base(settings) { }

        protected override float Process(FloatRemapSettings settings, float input, float time)
        {
            if (Math.Abs(settings.FromMax - settings.FromMin) < 1e-6)
            {
                return settings.ToMin;
            }
            float t = math.unlerp(settings.FromMin, settings.FromMax, input);
            float result = math.lerp(settings.ToMin, settings.ToMax, t);
            if (settings.Clamp)
            {
                result = math.clamp(result, math.min(settings.ToMin, settings.ToMax), math.max(settings.ToMin, settings.ToMax));
            }

            return result;
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