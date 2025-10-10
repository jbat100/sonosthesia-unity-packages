using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TouchEnvelopeSettings : IInteractiveEnvelopeSettings<TouchEvent>
    {
        [SerializeField] private bool _track;
        public bool Track => _track;
        
        [SerializeField] private EnvelopeFilter _filter;
        public EnvelopeFilter Filter => _filter;

        [SerializeField] private OneEuroFilterSettings _oneEuroFilter;
        public OneEuroFilterSettings OneEuroFilter => _oneEuroFilter;
        
        [SerializeField] private EnvelopeInteraction _interaction;
        public EnvelopeInteraction Interaction => _interaction;
        
        [SerializeField] private FloatTouchExtractorSettings _constantExtractor;
        public FloatExtractorSettings<TouchEvent> ConstantExtractor => _constantExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _valueScaleExtractor;
        public FloatExtractorSettings<TouchEvent> ValueScaleExtractor => _valueScaleExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _timeScaleExtractor;
        public FloatExtractorSettings<TouchEvent> TimeScaleExtractor => _timeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope
            = EnvelopeSettings.ADS(EnvelopePhase.InOutSine(0.1f), EnvelopePhase.InOutSine(0.3f), 0.5f);
        public EnvelopeSettings Envelope => _envelope;

        [SerializeField] private FloatTouchExtractorSettings _releaseExtractor;
        public FloatExtractorSettings<TouchEvent> ReleaseExtractor => _releaseExtractor;

        [SerializeField] private EaseType _releaseType = EaseType.easeInOutSine;
        public EaseType ReleaseType => _releaseType;
    }

    public static class TrackedTouchEnvelopeSettingsExtensions
    {
        public static IInteractiveEnvelopeSession<TouchEvent> SetupSession(this TouchEnvelopeSettings settings, TouchEvent e,
            TriggerController controller = null)
        {
            return InteractiveEnvelopeSessionUtil.StartSession(e, settings, controller);
        }
    }
}