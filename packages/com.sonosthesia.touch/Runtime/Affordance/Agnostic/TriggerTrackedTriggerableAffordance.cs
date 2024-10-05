using System;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerTrackedTriggerableAffordance : AgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerTrackedTriggerableAffordance>
    {
        [SerializeField] private TrackedTriggerable _triggerable;

        [Serializable]
        private class StartSettings
        {
            [SerializeField] private TriggerValueGenerator<float> _valueScaleGenerator;
            [SerializeField] private TriggerValueGenerator<float> _timeScaleGenerator;
            [SerializeField] private EnvelopeFactory _envelopeFactory;

            public Guid StartTrigger(TriggerTrackedTriggerableAffordance affordance, TriggerEvent e)
            {
                float valueScale = 1f;
                if (_valueScaleGenerator)
                {
                    _valueScaleGenerator.BeginTrigger(e.TriggerData, out valueScale);
                }
                float timeScale = 1f;
                if (_timeScaleGenerator)
                {
                    _timeScaleGenerator.BeginTrigger(e.TriggerData, out timeScale);
                }
                IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
                return affordance._triggerable.StartTrigger(envelope, valueScale, timeScale);
            }
        }

        [SerializeField] private StartSettings _start;
        
        [Serializable]
        private class EndSettings
        {
            [SerializeField] private TriggerValueGenerator<float> _timeScaleGenerator;
            [SerializeField] private EnvelopeFactory _envelopeFactory;

            public void EndTrigger(TriggerTrackedTriggerableAffordance affordance, Guid id, TriggerEvent e)
            {
                float timeScale = 1f;
                if (_timeScaleGenerator)
                {
                    _timeScaleGenerator.BeginTrigger(e.TriggerData, out timeScale);
                }
                IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
                affordance._triggerable.EndTrigger(envelope, id, timeScale);
            }
        }

        [SerializeField] private EndSettings _end;
        
        protected new class Controller : 
            AgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerTrackedTriggerableAffordance>.Controller
        {
            private Guid _triggerId;
            
            public Controller(Guid eventId, TriggerTrackedTriggerableAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TriggerEvent e)
            {
                base.Setup(e);
                
                TriggerTrackedTriggerableAffordance affordance = Affordance;

                if (!affordance._triggerable)
                {
                    if (affordance.Log)
                    {
                        Debug.Log($"{this} {nameof(Setup)} bailing out (no triggerable)");
                    }
                    return;
                }

                _triggerId = affordance._start.StartTrigger(affordance, e);
            }

            protected override void Teardown(TriggerEvent e)
            {
                base.Teardown(e);
                TriggerTrackedTriggerableAffordance affordance = Affordance;
                if (!affordance._triggerable)
                {
                    if (affordance.Log)
                    {
                        Debug.Log($"{this} {nameof(Teardown)} bailing out (no triggerable)");
                    }
                    return;
                }
                affordance._end.EndTrigger(affordance, _triggerId, e);
            }
        }

        protected override IObserver<TriggerEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}