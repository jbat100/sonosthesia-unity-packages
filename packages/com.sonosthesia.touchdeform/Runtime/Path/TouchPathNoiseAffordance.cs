using System;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
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
            
            // ~Controller()
            // {
            //     // checking for lifetime
            //     Debug.LogError("TouchPathNoiseAffordance Controller finalized and ready for garbage collection");
            // }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchPathNoiseAffordance affordance = Affordance;
                TouchPathNoiseConfiguration configuration = affordance._configuration;

                float3 center = e.TouchData.Actor.transform.position;

                TouchEnvelopeSession displacementSession = configuration.Displacement.SetupSession(e, out float displacementDuration);
                TouchEnvelopeSession radiusSession = configuration.Radius.SetupSession(e, out float radiusDuration);
                TouchEnvelopeSession frequencySession = configuration.Frequency.SetupSession(e, out float frequencyDuration);
                TouchEnvelopeSession speedSession = configuration.Speed.SetupSession(e, out float speedDuration);

                float duration = Mathf.Min(displacementDuration, radiusDuration, frequencyDuration, speedDuration);
                
                float time = 0f;
                
                void Cleanup()
                {
                    affordance._processor.Unregister(EventId);
                }
                
                Observable.EveryUpdate()
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration)))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        time += Time.deltaTime * speedSession.Update();
                        CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                            affordance._configuration.NoiseType,
                            displacementSession.Update(),
                            affordance._configuration.FalloffType,
                            center,
                            radiusSession.Update(),
                            time,
                            float3.zero,
                            frequencySession.Update()
                        );
                        affordance._processor.Register(EventId, info);
                    }, err => Cleanup(), Cleanup);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
    }
}