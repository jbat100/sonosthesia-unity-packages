using System;
using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatOneEuroFilterSettings : DynamicProcessorSettings
    {
        [SerializeField] private float _beta = 1f;
        public float Beta => _beta;

        [SerializeField] private float _minCutoff = 1f;
        public float MinCutoff => _minCutoff;
    }

    public class FloatOneEuroFilterProcessor : DynamicProcessor<float, FloatOneEuroFilterSettings>
    {
        private OneEuroFilter2 _filter = new();
        
        public FloatOneEuroFilterProcessor(FloatOneEuroFilterSettings settings) : base(settings) { }

        protected override float Process(FloatOneEuroFilterSettings settings, float input, float time)
        {
            _filter.Beta = settings.Beta;
            _filter.MinCutoff = settings.MinCutoff;
            return _filter.Step(time, new float2(input, 0f)).x;
        }

        public override void Reset()
        {
            _filter = new OneEuroFilter2();
        }
    }

    
    [CreateAssetMenu(fileName = "FloatOneEuroFilter", menuName = "Sonosthesia/Processing/FloatOneEuroFilter")]
    public class FloatOneEuroFilterProcessorFactory 
        : ConfigurableDynamicProcessorFactory<FloatOneEuroFilterSettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatOneEuroFilterSettings settings)
            => new FloatOneEuroFilterProcessor(settings);
    }
}