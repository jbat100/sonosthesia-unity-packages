using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TouchEnvelopeSettings
    {
        public enum TouchType
        {
            Constant,
            Pulse,
            Contact
        }
        
        [SerializeField] private TouchType _type;
        public TouchType Type => _type;
        
        [SerializeField] private FloatTouchExtractorSettings _constantExtractor;
        public FloatTouchExtractorSettings ConstantExtractor => _constantExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _valueScaleExtractor;
        public FloatTouchExtractorSettings ValueScaleExtractor => _valueScaleExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _timeScaleExtractor;
        public FloatTouchExtractorSettings TimeScaleExtractor => _timeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope
            = EnvelopeSettings.ADS(EnvelopePhase.Linear(0.5f), EnvelopePhase.Linear(0.5f), 0.5f);
        public EnvelopeSettings Envelope => _envelope;

        [SerializeField] private bool _trackValue;
        public bool TrackValue => _trackValue;
        
        [SerializeField] private FloatTouchExtractorSettings _releaseExtractor;
        public FloatTouchExtractorSettings ReleaseExtractor => _releaseExtractor;

        [SerializeField] private EaseType _releaseType = EaseType.linear;
        public EaseType ReleaseType => _releaseType;
    }

    public static class TrackedTouchEnvelopeSettingsExtensions
    {
        public static ITouchEnvelopeSession SetupSession(this TouchEnvelopeSettings settings, TouchEvent e,
            TriggerController controller = null)
        {
            return TouchEnvelopeSessionUtil.StartSession(e, settings, controller);
        }
    }
}