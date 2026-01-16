using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatProcessorSettings : IProcessor<float>
    {
        [SerializeField] private FloatProcessingType _processing;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;
        [SerializeField] private float _randomization;
        
        public float Process(float input) => _processing.ProcessFloat(input, _curve, _remap, _clamp, _randomization);
    }
}