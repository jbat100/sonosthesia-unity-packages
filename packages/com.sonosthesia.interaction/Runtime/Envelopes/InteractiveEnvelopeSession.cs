using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    internal class InteractiveEnvelopeImplementation
    {
        private class PhaseInfo
        {
            private static float CurrentTime => Time.time;
            
            public float ReferenceTime;
            public IEnvelope Envelope;

            public bool IsComplete => CurrentTime - ReferenceTime > Envelope.Duration;

            public float Evaluate()
            {
                return IsComplete ? Envelope.FinalValue : Envelope.Evaluate(CurrentTime - ReferenceTime);
            }
        }

        private readonly PhaseInfo _start;
        private PhaseInfo _end;

        private PhaseInfo CurrentPhase => _end ?? _start;
        
        public InteractiveEnvelopeImplementation(IEnvelope envelope, float value)
        {
            Value = value;
            _start = new PhaseInfo
            {
                ReferenceTime = Time.time,
                Envelope = envelope
            };
        }

        public float Evaluate() => Value * CurrentPhase.Evaluate();
        
        public float Value;

        public void End(IEnvelope envelope)
        {
            if (_end != null)
            {
                return;
            }

            float valueScale = envelope.InitialValue == 0f ? 1f : _start.Evaluate() / envelope.InitialValue;
            
            _end = new PhaseInfo
            {
                ReferenceTime = Time.time,
                Envelope = new WarpedEnvelope(envelope, valueScale, 1f)
            };
        }
    }
    
    public static class InteractiveEnvelopeSessionUtil
    {
        public static IInteractiveEnvelopeSession<TEvent> StartSession<TEvent>(TEvent e, 
            IInteractiveEnvelopeSettings<TEvent> settings)
        {
            IInteractiveEnvelopeSession<TEvent> session = MakeSession(settings);
            session.Start(e);
            return session;
        }
        
        public static IInteractiveEnvelopeSession<TEvent> StartSession<TEvent>(this IInteractiveEnvelopeSettings<TEvent> settings, TEvent e)
        {
            return StartSession(e, settings);
        }

        public static IInteractiveEnvelopeSession<TEvent> MakeSession<TEvent>(IInteractiveEnvelopeSettings<TEvent> s)
        {
            IInteractiveEnvelopeSession<TEvent> session = s.Interaction switch
            {
                EnvelopeInteraction.Bypass => new BypassEnvelopeSession<TEvent>(s.ValueExtractor),
                EnvelopeInteraction.Pulse or EnvelopeInteraction.Hold => new EnvelopeSession<TEvent>(s),
                _ => throw new ArgumentOutOfRangeException()
            };   
            if (s.Filter == EnvelopeFilter.OneEuro)
            {
                session = new StaticInteractiveEnvelopeSessionOneEuroFilter<TEvent>(session, s.OneEuroFilter);
            }

            return session;
        }

        private class BypassEnvelopeSession<TEvent> : IInteractiveEnvelopeSession<TEvent> 
        {
            private readonly IDynamicExtractor<TEvent, float> _extractor;
            private IDynamicExtractorSession<TEvent, float> _session;
            
            private float _value;

            public BypassEnvelopeSession(IDynamicExtractor<TEvent, float> extractor)
            {
                _extractor = extractor;
            }

            public void Start(TEvent e)
            {
                _session = _extractor.MakeSession();
                if (!_session.Setup(e, out _value))
                {
                    _value = 0f;
                }
            }

            public void Update(TEvent e)
            {
                _session.Update(e, out _value);
            }

            public void End(TEvent e, out float release)
            {
                release = 0;
            }

            public float Evaluate() => _value;
        }


        private class EnvelopeSession<TEvent> : IInteractiveEnvelopeSession<TEvent>
        {
            private readonly IInteractiveEnvelopeSettings<TEvent> _settings;
            private readonly bool _autoRelease;
            
            private IDynamicExtractorSession<TEvent, float> _valueSession;
            private InteractiveEnvelopeImplementation _implementation;
            private float _endTime;

            public EnvelopeSession(IInteractiveEnvelopeSettings<TEvent> settings)
            {
                _settings = settings;
                _autoRelease = settings.Interaction == EnvelopeInteraction.Pulse;
            }
            
            public void Start(TEvent e)
            {
                _valueSession = _settings.ValueExtractor.MakeSession();
                if (!_valueSession.Setup(e, out float value))
                {
                    value = 1f;
                }
                if (!_settings.AttackExtractor.Extract(e, out float attack))
                {
                    attack = 1f;
                }
                IEnvelope envelope = new WarpedEnvelope(_settings.Envelope.Build(), 1f, attack);
                _endTime = Time.time + envelope.Duration;
                _implementation = new InteractiveEnvelopeImplementation(envelope, value);
            }

            public void Update(TEvent e)
            {
                if (_valueSession.Update(e, out float value))
                {
                    _implementation.Value = value;   
                }
            }
            
            public void End(TEvent e, out float release)
            {
                if (_autoRelease)
                {
                    // make sure pulse has time to complete
                    release = Mathf.Max(0, _endTime - Time.time);
                }
                else
                {
                    if (!_settings.ReleaseExtractor.Extract(e, out release))
                    {
                        release = 1f;
                    }
                    _implementation.End(_settings.ReleaseType.ReleaseEnvelope(release));
                }
            }
            
            public float Evaluate() => _implementation.Evaluate();
        }
    }
}