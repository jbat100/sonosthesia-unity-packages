using System;
using Sonosthesia.Deform;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using Sonosthesia.Trigger;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    public class TouchPathNoiseAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private TouchPathNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoisePathProcessor _processor;

        private class Controller : AffordanceController<TouchEvent, TouchPathNoiseAffordance>
        {
            public Controller(Guid eventId, TouchPathNoiseAffordance affordance) : base(eventId, affordance)
            {
            }
            
            ~Controller()
            {
                // checking for lifetime
                Debug.LogError("TouchPathNoiseAffordance Controller finalized and ready for garbage collection");
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchPathNoiseAffordance affordance = Affordance;

                if (!affordance._configuration)
                {
                    return;
                }
                
                float3 center = e.TouchData.Actor.transform.position;
                
                ITouchExtractorSession<float> displacementValueSession = affordance._configuration.DisplacementValueExtractor.MakeSession();
                displacementValueSession.Setup(e, out float valueScale);
                
                ITouchExtractorSession<float> displacementTimeSession = affordance._configuration.DisplacementTimeExtractor.MakeSession();
                displacementTimeSession.Setup(e, out float timeScale);

                IEnvelope envelope = affordance._configuration.DisplacementEnvelope.Build();
                
                ITouchExtractorSession<float> radiusSession = affordance._configuration.RadiusExtractor.MakeSession();
                radiusSession.Setup(e, out float radius);

                TriggerController triggerController = new TriggerController(AccumulationMode.Max);

                Observable.EveryUpdate()
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(envelope.Duration * timeScale)))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        float displacement = triggerController.Update();
                        CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                            affordance._configuration.NoiseType,
                            displacement,
                            affordance._configuration.FalloffType,
                            center,
                            radius,
                            Time.time,
                            float3.zero,
                            affordance._configuration.Frequency
                        );
                        affordance._processor.Register(EventId, info);
                    }, err =>
                    {
                        affordance._processor.Unregister(EventId);
                    }, () =>
                    {
                        affordance._processor.Unregister(EventId);
                    });
            }
        }
        
        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}