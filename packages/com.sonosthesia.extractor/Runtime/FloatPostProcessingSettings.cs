using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Extractor
{
    [Serializable]
    public class FloatPostProcessingSettings : IPostProcessing<float>
    {
        [SerializeField] private FloatProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;
        
        public float PostProcess(float input) => _postProcessing.ProcessFloat(input, _curve, _remap, _clamp);
    }
}