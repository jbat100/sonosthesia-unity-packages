using System;
using UnityEngine;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    public class TouchTrackedTriggerAffordance : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>
    {
        [SerializeField] private TrackedTrigger _trigger;

        [SerializeField] private bool _track;

        [Serializable]
        private class StartSettings
        {
            [SerializeField] private TouchExtractor<float> _valueScaleExtractor;
            public TouchExtractor<float> ValueScaleExtractor => _valueScaleExtractor;
            
            [SerializeField] private TouchExtractor<float> _timeScaleExtractor;
            public TouchExtractor<float> TimeScaleExtractor => _timeScaleExtractor;
            
            [SerializeField] private EnvelopeFactory _envelopeFactory;
            public EnvelopeFactory EnvelopeFactory => _envelopeFactory;
        }

        [SerializeField] private StartSettings _start;
        
        [Serializable]
        private class EndSettings
        {
            [SerializeField] private TouchExtractor<float> _timeScaleExtractor;
            public TouchExtractor<float> TimeScaleExtractor => _timeScaleExtractor;
            
            [SerializeField] private EnvelopeFactory _envelopeFactory;
            public EnvelopeFactory EnvelopeFactory => _envelopeFactory;
        }

        [SerializeField] private EndSettings _end;
        
        protected new class Controller : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>.Controller
        {
            private Guid _triggerId;

            private ITouchExtractorSession<float> _valueScaleSession;
            
            public Controller(Guid eventId, TouchTrackedTriggerAffordance affordance) : base(eventId, affordance)
            {
                
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchTrackedTriggerAffordance affordance = Affordance;
                
                if (affordance._start == null)
                {
                    _triggerId = affordance._trigger.StartTrigger(1f, 1f);
                    return;
                }

                _valueScaleSession = affordance._start.ValueScaleExtractor.MakeSession(); 
                ITouchExtractorSession<float> timeScaleSession = affordance._start.TimeScaleExtractor.MakeSession();
                IEnvelope envelope = affordance._start.EnvelopeFactory ? affordance._start.EnvelopeFactory.Build() : null;

                if (!(_valueScaleSession?.Setup(e, out float valueScale) ?? false))
                {
                    valueScale = 1f;
                }
                if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
                {
                    timeScale = 1f;
                }
                _triggerId = affordance._trigger.StartTrigger(envelope, valueScale, timeScale);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                TouchTrackedTriggerAffordance affordance = Affordance;

                if (!affordance._track)
                {
                    return;
                }

                if (_valueScaleSession == null)
                {
                    return;
                }

                if (!_valueScaleSession.Update(e, out float value))
                {
                    return;
                }

                Affordance._trigger.UpdateTrigger(_triggerId, value);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchTrackedTriggerAffordance affordance = Affordance;

                if (affordance._end == null)
                {
                    affordance._trigger.EndTrigger(_triggerId);
                    return;
                }
                
                ITouchExtractorSession<float> timeScaleSession = affordance._end.TimeScaleExtractor.MakeSession();
                if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
                {
                    timeScale = 1f;
                }
                IEnvelope envelope = affordance._end.EnvelopeFactory ? affordance._start.EnvelopeFactory.Build() : null;
                affordance._trigger.EndTrigger(_triggerId, envelope, timeScale);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}