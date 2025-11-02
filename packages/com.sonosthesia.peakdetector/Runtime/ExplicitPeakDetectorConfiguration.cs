using System;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.PeakDetector
{
    [Obsolete("Use PeakDetectorConfiguration")]
    [CreateAssetMenu(fileName = "PeakDetector", menuName = "Sonosthesia/Obsolete/PeakDetector")]
    public class ExplicitPeakDetectorConfiguration : BasePeakDetectorConfiguration
    {
        [SerializeField] private float _magnitudeThreshold = .1f;
        public float MagnitudeThreshold => _magnitudeThreshold;
        
        [SerializeField] private float _maximumDuration = .1f;
        public float MaximumDuration => _maximumDuration;

        [SerializeField] private FloatProcessorSettings _valuePostProcessor;
        public FloatProcessorSettings ValuePostProcessor => _valuePostProcessor;
        
        public override PeakDetectorSettings Settings => new (_magnitudeThreshold, _maximumDuration, _valuePostProcessor);
    }
}