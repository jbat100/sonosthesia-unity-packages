using System;
using UnityEngine;
using Sonosthesia.Envelope;

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
        public static void TriggerParameters(this TouchEnvelopeSettings settings, TouchEvent touchEvent,
            out IEnvelope envelope, out float valueScale, out float timeScale)
        {
            ITouchExtractorSession<float> valueScaleSession = settings.ValueScaleExtractor.MakeSession(); 
            ITouchExtractorSession<float> timeScaleSession = settings.TimeScaleExtractor.MakeSession();
            envelope = settings.Envelope.Build();

            if (!(valueScaleSession?.Setup(touchEvent, out valueScale) ?? false))
            {
                valueScale = 1f;
            }
            if (!(timeScaleSession?.Setup(touchEvent, out timeScale) ?? false))
            {
                timeScale = 1f;
            }
        }

        public static TouchEnvelopeSession SetupSession(this TouchEnvelopeSettings settings, TouchEvent touchEvent,
            out float duration)
        {
            TouchEnvelopeSession session = new TouchEnvelopeSession(settings);
            session.Setup(touchEvent, out duration);
            return session;
        }
    }
}