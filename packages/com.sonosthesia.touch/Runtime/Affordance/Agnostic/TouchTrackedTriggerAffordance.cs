using System;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTrackedTriggerAffordance : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>
    {
        [SerializeField] private TrackedTrigger trigger;

        [Serializable]
        private class StartSettings
        {
            [SerializeField] private TouchValueGenerator<float> _valueScaleGenerator;
            [SerializeField] private TouchValueGenerator<float> _timeScaleGenerator;
            [SerializeField] private EnvelopeFactory _envelopeFactory;

            public Guid StartTrigger(TouchTrackedTriggerAffordance affordance, TouchEvent e)
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

            public void EndTrigger(TouchTrackedTriggerAffordance affordance, Guid id, TouchEvent e)
            {
                float timeScale = 1f;
                if (_timeScaleGenerator)
                {
                    _timeScaleGenerator.BeginTouch(e.TouchData, out timeScale);
                }
                IEnvelope envelope = _envelopeFactory ? _envelopeFactory.Build() : null;
                affordance.trigger.EndTrigger(envelope, id, timeScale);
            }
        }

        [SerializeField] private EndSettings _end;
        
        protected new class Controller : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>.Controller
        {
            private Guid _triggerId;
            
            public Controller(Guid eventId, TouchTrackedTriggerAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);
                
                TouchTrackedTriggerAffordance affordance = Affordance;

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
                TouchTrackedTriggerAffordance affordance = Affordance;
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