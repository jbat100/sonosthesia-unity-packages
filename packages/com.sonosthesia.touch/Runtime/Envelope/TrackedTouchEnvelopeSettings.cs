using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TrackedTouchEnvelopeSettings
    {
        [SerializeField] private FloatTouchExtractorSettings _valueScaleExtractor;
        public FloatTouchExtractorSettings ValueScaleExtractor => _valueScaleExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _timeScaleExtractor;
        public FloatTouchExtractorSettings TimeScaleExtractor => _timeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope
            = EnvelopeSettings.ADS(EnvelopePhase.Linear(0.5f), EnvelopePhase.Linear(0.5f), 0.5f);
        public EnvelopeSettings Envelope => _envelope;

        [SerializeField] private bool _track;
        public bool Track => _track;
        
        [SerializeField] private FloatTouchExtractorSettings _releaseExtractor;
        public FloatTouchExtractorSettings ReleaseExtractor => _releaseExtractor;

        [SerializeField] private EaseType _releaseType = EaseType.linear;
        public EaseType ReleaseType => _releaseType;
    }

    public static class TrackedTouchEnvelopeSettingsExtensions
    {
        public static TrackedTouchEnvelopeSession MakeSession(this TrackedTouchEnvelopeSettings settings)
        {
            return new TrackedTouchEnvelopeSession(settings);
        }

        public static TrackedTouchEnvelopeSession SetupSession(this TrackedTouchEnvelopeSettings settings, TouchEvent e)
        {
            TrackedTouchEnvelopeSession session = settings.MakeSession();
            session.StartTouch(e);
            return session;
        }
        
        public static IEnvelope ReleaseEnvelope(this TrackedTouchEnvelopeSettings settings, TouchEvent e)
        {
            if (!settings.ReleaseExtractor.Extract(e, out float release))
            {
                release = 1f;
            }
            return settings.ReleaseType.ReleaseEnvelope(release);
        }
    }
}