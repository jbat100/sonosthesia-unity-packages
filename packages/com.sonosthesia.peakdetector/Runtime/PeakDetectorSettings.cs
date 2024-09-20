using System;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.PeakDetector
{
    [Serializable]
    public class PeakDetectorSettings
    {
        [SerializeField] private float _magnitudeThreshold = .1f;
        public float MagnitudeThreshold => _magnitudeThreshold;
        
        [SerializeField] private float _maximumDuration = .1f;
        public float MaximumDuration => _maximumDuration;

        [SerializeField] private FloatProcessor _valuePostProcessor;
        public FloatProcessor ValuePostProcessor => _valuePostProcessor;

        public PeakDetectorSettings(float magnitudeThreshold, float maximumDuration, FloatProcessor valuePostProcessor)
        {
            _magnitudeThreshold = magnitudeThreshold;
            _maximumDuration = maximumDuration;
            _valuePostProcessor = valuePostProcessor;
        }
    }

    [Serializable]
    public class PreprocessedPeakDetectorSettings
    {
        [SerializeField] private PeakDetectorSettings _settings;
        private PeakDetectorSettings Settings => _settings;

        [SerializeField] private DynamicProcessorFactory<float> _preprocessorFactory;
        private DynamicProcessorFactory<float> PreprocessorFactory => _preprocessorFactory;

        internal PeakDetectorImplementation MakeImplementation(Action<Peak> broadcast, bool log = false)
        {
            return new PeakDetectorImplementation(_preprocessorFactory ? _preprocessorFactory.Make() : null, _settings, broadcast, log);
        }
    }
}