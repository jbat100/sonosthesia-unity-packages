using System;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    
    [Obsolete("Use Extractors instead of Generators")]
    public class GeneratorTouchTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private Trigger.Trigger trigger;

        [SerializeField] private TouchValueGenerator<float> _valueScaleGenerator;
        
        [SerializeField] private TouchValueGenerator<float> _timeScaleGenerator;

        [SerializeField] private EnvelopeFactory _envelopeFactory;
        
        private class Controller : AffordanceController<TouchEvent, GeneratorTouchTriggerAffordance>
        {
            public Controller(Guid eventId, GeneratorTouchTriggerAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);
                
                GeneratorTouchTriggerAffordance affordance = Affordance;

                if (!affordance.trigger)
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
                    affordance._valueScaleGenerator.BeginTouch(e.TouchData, out valueScale);
                }
                float timeScale = 1f;
                if (affordance._timeScaleGenerator)
                {
                    affordance._timeScaleGenerator.BeginTouch(e.TouchData, out timeScale);
                }

                IEnvelope envelope = affordance._envelopeFactory ? affordance._envelopeFactory.Build() : null;
                
                affordance.trigger.StartTrigger(envelope, valueScale, timeScale);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                GeneratorTouchTriggerAffordance affordance = Affordance;
                if (affordance._valueScaleGenerator)
                {
                    affordance._valueScaleGenerator.EndTouch(e.TouchData);
                }
                if (affordance._timeScaleGenerator)
                {
                    affordance._timeScaleGenerator.EndTouch(e.TouchData);
                }
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}