using System;
using UnityEngine;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TouchEnvelopeSettings
    {
        [SerializeField] private FloatTouchExtractorSettings _valueScaleExtractor;
        public FloatTouchExtractorSettings ValueScaleExtractor => _valueScaleExtractor;

        [SerializeField] private FloatTouchExtractorSettings _timeScaleExtractor;
        public FloatTouchExtractorSettings TimeScaleExtractor => _timeScaleExtractor;

        [SerializeField] private EnvelopeSettings _envelope;
        public EnvelopeSettings Envelope => _envelope;
    }

    public static class TouchEnvelopeSettingsExtensions
    {
        public static TouchEnvelopeSession SetupSession(this TouchEnvelopeSettings settings, TouchEvent touchEvent,
            out float duration, TriggerController controller = null)
        {
            TouchEnvelopeSession session = new TouchEnvelopeSession(settings, controller);
            session.Setup(touchEvent, out duration);
            return session;
        }
    }
}