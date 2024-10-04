using System;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerTriggerableAffordance : AgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerTriggerableAffordance>
    {
        [SerializeField] private Triggerable _triggerable;

        [SerializeField] private TriggerValueGenerator<float> _valueScaleGenerator;
        
        [SerializeField] private TriggerValueGenerator<float> _timeScaleGenerator;

        [SerializeField] private EnvelopeFactory _envelopeFactory;
        
        protected new class Controller : 
            AgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerTriggerableAffordance>.Controller
        {
            public Controller(Guid eventId, TriggerTriggerableAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TriggerEvent e)
            {
                base.Setup(e);
                
                TriggerTriggerableAffordance affordance = Affordance;

                if (!affordance._triggerable)
                {
                    if (affordance.Log)
                    {
                        Debug.Log($"{this} {nameof(Setup)} bailing out (no triggerable)");
                    }
                    return;
                }
                
                float valueScale = 1f;
                if (affordance._valueScaleGenerator)
                {
                    affordance._valueScaleGenerator.BeginTrigger(e.TriggerData, out valueScale);
                }
                float timeScale = 1f;
                if (affordance._timeScaleGenerator)
                {
                    affordance._timeScaleGenerator.BeginTrigger(e.TriggerData, out timeScale);
                }

                IEnvelope envelope = affordance._envelopeFactory ? affordance._envelopeFactory.Build() : null;
                
                affordance._triggerable.Trigger(valueScale, timeScale);
            }

            protected override void Teardown(TriggerEvent e)
            {
                base.Teardown(e);
                TriggerTriggerableAffordance affordance = Affordance;
                if (affordance._valueScaleGenerator)
                {
                    affordance._valueScaleGenerator.EndTrigger(e.TriggerData);
                }
                if (affordance._timeScaleGenerator)
                {
                    affordance._timeScaleGenerator.EndTrigger(e.TriggerData);
                }
            }
        }

        protected override IObserver<TriggerEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}