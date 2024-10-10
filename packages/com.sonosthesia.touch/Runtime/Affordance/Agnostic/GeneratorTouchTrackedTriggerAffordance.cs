using System;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    // mostly for testing purposes, use configuration ScriptableObject
    
    [Obsolete("Use Extractors instead of Generators")]
    public class GeneratorTouchTrackedTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private TrackedTrigger trigger;

        [Serializable]
        private class StartSettings
        {
            [SerializeField] private TouchValueGenerator<float> _valueScaleGenerator;
            [SerializeField] private TouchValueGenerator<float> _timeScaleGenerator;
            [SerializeField] private EnvelopeFactory _envelopeFactory;

            public Guid StartTrigger(GeneratorTouchTrackedTriggerAffordance affordance, TouchEvent e)
            {
                float valueScale = 1f;
                if (_valueScaleGenerator)
                {
                    _valueScaleGenerator.BeginTouch(e.TouchData, out valueScale);
                }
                float timeScale = 1f;
                if (_timeScaleGenerator)
                {
                    _timeScaleGenerator.BeginTouch(e.TouchData, out timeScale);
                }
                IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
                return affordance.trigger.StartTrigger(envelope, valueScale, timeScale);
            }
        }

        [SerializeField] private StartSettings _start;
        
        [Serializable]
        private class EndSettings
        {
            [SerializeField] private TouchValueGenerator<float> _timeScaleGenerator;
            [SerializeField] private EnvelopeFactory _envelopeFactory;

            public void EndTrigger(GeneratorTouchTrackedTriggerAffordance affordance, Guid id, TouchEvent e)
            {
                float timeScale = 1f;
                if (_timeScaleGenerator)
                {
                    _timeScaleGenerator.BeginTouch(e.TouchData, out timeScale);
                }
                IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
                affordance.trigger.EndTrigger(id, envelope, timeScale);
            }
        }

        [SerializeField] private EndSettings _end;
        
        private class Controller : AffordanceController<TouchEvent, GeneratorTouchTrackedTriggerAffordance>
        {
            private Guid _triggerId;
            
            public Controller(Guid eventId, GeneratorTouchTrackedTriggerAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);
                
                GeneratorTouchTrackedTriggerAffordance affordance = Affordance;

                if (!affordance.trigger)
                {
                    if (affordance.Log)
                    {
                        Debug.Log($"{this} {nameof(Setup)} bailing out (no triggerable)");
                    }
                    return;
                }

                _triggerId = affordance._start.StartTrigger(affordance, e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                GeneratorTouchTrackedTriggerAffordance affordance = Affordance;
                if (!affordance.trigger)
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

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}