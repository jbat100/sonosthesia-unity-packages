using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public static class InteractiveTriggerSessionUtils
    {
        public static IInteractiveTriggerSession<TEvent> StartSession<TEvent>(
            TEvent e, IInteractiveTriggerSettings<TEvent> settings, TriggerImplementation trigger)
        {
            IInteractiveTriggerSession<TEvent> session = MakeSession(settings, trigger);
            session.Start(e);
            return session;
        }
        
        public static IInteractiveTriggerSession<TEvent> StartSession<TEvent>(
            this IInteractiveTriggerSettings<TEvent> settings, TEvent e, TriggerImplementation trigger)
        {
            return StartSession(e, settings, trigger);
        }

        public static IInteractiveTriggerSession<TEvent> MakeSession<TEvent>(
            IInteractiveTriggerSettings<TEvent> s, TriggerImplementation trigger)
        {
            IInteractiveTriggerSession<TEvent> session = s.Interaction switch
            {
                TriggerInteraction.Pulse => new PulseTriggerSession<TEvent>(trigger, s.Envelope, s.AttackExtractor, s.ValueExtractor),
                TriggerInteraction.Hold => new HoldEnvelopeSession<TEvent>(trigger, s.Envelope, s.AttackExtractor, s.ReleaseExtractor, s.ReleaseType, s.ValueExtractor),
                _ => throw new ArgumentOutOfRangeException()
            };
            return session;
        }
        
        private class PulseTriggerSession<TEvent> : IInteractiveTriggerSession<TEvent> 
        {
            private readonly Guid _id = Guid.NewGuid();
            private readonly EnvelopeSettings _envelope;
            private readonly IStaticExtractor<TEvent, float> _attackExtractor;
            private readonly IDynamicExtractor<TEvent, float> _valueExtractor;
            private readonly TriggerImplementation _implementation;
            
            private IDynamicExtractorSession<TEvent, float> _valueSession;
            private float _endTime;
            
            public PulseTriggerSession(TriggerImplementation implementation,
                EnvelopeSettings envelope, IStaticExtractor<TEvent, float> attackExtractor, 
                IDynamicExtractor<TEvent, float> valueExtractor)
            {
                _envelope = envelope;
                _attackExtractor = attackExtractor;
                _valueExtractor = valueExtractor;
                _implementation = implementation;
            }
            
            public void Start(TEvent e)
            {
                _valueSession = _valueExtractor.MakeSession();
                if (!_valueSession.Setup(e, out float value))
                {
                    value = 1f;
                }
                if (!_attackExtractor.Extract(e, out float attack))
                {
                    attack = 1f;
                }
                IEnvelope envelope = _envelope.Build();
                _endTime = Time.time + envelope.Duration * attack;
                _implementation.StartTrigger(_id, _envelope.Build(), value, attack, true);
            }

            public void Update(TEvent e)
            {
                if (_valueSession.Update(e, out float value))
                {
                    _implementation.UpdateTrigger(_id, value);   
                }
            }
            
            public void End(TEvent e, out float release)
            {
                // make sure pulse has time to complete
                release = Mathf.Max(0, _endTime - Time.time);
            }
        }
        
        private class HoldEnvelopeSession<TEvent> : IInteractiveTriggerSession<TEvent> 
        {
            private readonly Guid _id = Guid.NewGuid();
            private readonly EnvelopeSettings _envelope;
            private readonly IStaticExtractor<TEvent, float> _attackExtractor;
            private readonly IStaticExtractor<TEvent, float> _releaseExtractor;
            private readonly EaseType _releaseType;
            private readonly IDynamicExtractor<TEvent, float> _valueExtractor;
            
            private IDynamicExtractorSession<TEvent, float> _valueSession;
            private TriggerImplementation _implementation;

            public HoldEnvelopeSession(TriggerImplementation implementation,
                EnvelopeSettings envelope, IStaticExtractor<TEvent, float> attackExtractor, 
                IStaticExtractor<TEvent, float> releaseExtractor, EaseType releaseType, 
                IDynamicExtractor<TEvent, float> valueExtractor)
            {
                _envelope = envelope;
                _attackExtractor = attackExtractor;
                _releaseExtractor = releaseExtractor;
                _releaseType = releaseType;
                _valueExtractor = valueExtractor;
            }
            
            public void Start(TEvent e)
            {
                _valueSession = _valueExtractor.MakeSession(); 
                if (!_valueSession.Setup(e, out float value))
                {
                    value = 1f;
                }
                if (!_attackExtractor.Extract(e, out float attack))
                {
                    attack = 1f;
                }
                _implementation.StartTrigger(_id, _envelope.Build(), value, attack, false);
            }

            public void Update(TEvent e)
            {
                if (_valueSession.Update(e, out float value))
                {
                    _implementation.UpdateTrigger(_id, value);
                }
            }

            public void End(TEvent e, out float release)
            {
                if (!_releaseExtractor.Extract(e, out release))
                {
                    release = 1f;
                }
                _implementation.EndTrigger(_id, _releaseType.ReleaseEnvelope(release), 1f);
            }
        }
    }
}