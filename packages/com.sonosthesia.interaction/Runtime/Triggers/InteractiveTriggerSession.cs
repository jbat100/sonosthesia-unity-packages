using System;
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
            return new TriggerSession<TEvent>(trigger, s);
        }
        
        private class TriggerSession<TEvent> : IInteractiveTriggerSession<TEvent> 
        {
            private readonly Guid _id = Guid.NewGuid();
            private readonly IInteractiveTriggerSettings<TEvent> _settings;
            private readonly TriggerImplementation _implementation;
            private readonly bool _autoRelease;
            
            private IDynamicExtractorSession<TEvent, float> _valueSession;
            private float _endTime;

            public TriggerSession(TriggerImplementation implementation, IInteractiveTriggerSettings<TEvent> settings)
            {
                _settings = settings;
                _implementation = implementation;
                _autoRelease = _settings.Interaction == TriggerInteraction.Pulse;
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
                IEnvelope envelope = _settings.Envelope.Build();
                _endTime = Time.time + envelope.Duration * attack;
                _implementation.StartTrigger(_id, envelope, value, attack, _autoRelease);
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
                    _implementation.EndTrigger(_id, _settings.ReleaseType.ReleaseEnvelope(release), 1f);
                }
            }
        }
    }
}