using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatLogRangeSettings : DynamicProcessorSettings
    {
        [SerializeField] private float _min = 20f;
        public float Min => _min;

        [SerializeField] private float _center = 500f;
        public float Center => _center;
        
        [SerializeField] private float _max = 15000;
        public float Max => _max;
    }
    
    public class FloatLogRangeProcessor : DynamicProcessor<float, FloatLogRangeSettings>
    {
        public FloatLogRangeProcessor(FloatLogRangeSettings settings) : base(settings) { }

        protected override float Process(FloatLogRangeSettings settings, float input, float time)
        {
            // TODO : consider creating converter in the settings to avoid recreating on each call (expensive)
            LogRangeConverter converter = new LogRangeConverter(settings.Min, settings.Center, settings.Max);
            return converter.ToRange(input);
        }
    }
    /// <summary>
    /// Useful for converting [0-1] range to audible frequency range
    /// </summary>
    [CreateAssetMenu(fileName = "FloatLogRange", menuName = "Sonosthesia/Processing/FloatLogRange")]
    public class FloatLogRangeProcessorFactory : ConfigurableDynamicProcessorFactory<FloatLogRangeSettings, float>
    {
        protected override IDynamicProcessor<float> Make(FloatLogRangeSettings settings) => 
            new FloatLogRangeProcessor(settings);
    }
}