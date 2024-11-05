using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public enum FloatModulationStrategy
    {
        None,
        Add,
        Multiply,
        Max,
        Min
    }
    
    public static class FloatModulationExtensions
    {
        public static float Modulate(this FloatModulationStrategy strategy, IEnumerable<float> values)
        {
            return strategy switch
            {
                FloatModulationStrategy.Add => values.Sum(),
                FloatModulationStrategy.Multiply => values.Aggregate(1.0f, (current, val) => current * val),
                FloatModulationStrategy.Min => values.Min(),
                FloatModulationStrategy.Max => values.Max(),
                _ => 0
            };
        }
        
        public static float Modulate(this FloatModulationStrategy strategy, float first, float second)
        {
            return strategy switch
            {
                FloatModulationStrategy.Add => first + second,
                FloatModulationStrategy.Multiply => first * second,
                FloatModulationStrategy.Min => math.min(first, second),
                FloatModulationStrategy.Max => math.max(first, second),
                _ => 0
            };
        }
    }

    [Serializable]
    public class FloatModulationSettings
    {
        [SerializeField] private FloatModulationStrategy _strategy;
        [SerializeField] private float _scale = 1f;
        [SerializeField] private float _offset = 0f;

        public float Modulate(float value, float modulator)
        {
            return _strategy.Modulate(value, modulator * _scale + _offset);
        }
    }
}