using System;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using Sonosthesia.Noise;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    public class TouchMeshNoiseAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private TouchMeshNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoiseMeshController _processor;
        
        private class Controller : AffordanceController<TouchEvent, TouchMeshNoiseAffordance>
        {
            public Controller(Guid eventId, TouchMeshNoiseAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchMeshNoiseAffordance affordance = Affordance;
                TouchMeshNoiseConfiguration configuration = affordance._configuration;

                float3 center = e.TouchData.Actor.transform.position;

                TouchEnvelopeSession displacementSession = configuration.Displacement.SetupSession(e, out float displacementDuration);
                TouchEnvelopeSession radiusSession = configuration.Radius.SetupSession(e, out float radiusDuration);
                TouchEnvelopeSession speedSession = configuration.Speed.SetupSession(e, out float speedDuration);

                float duration = Mathf.Min(displacementDuration, radiusDuration, speedDuration);
                
                float time = 0f;
                float3x4 rts = (new SpaceTRS { scale = 1 }).Matrix;
                
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
                        CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                            affordance._configuration.CrossFadeType,
                            affordance._configuration.NoiseType,
                            displacementSession.Update(),
                            rts,
                            affordance._configuration.Falloff,
                            affordance._configuration.FalloffType,
                            center,
                            radiusSession.Update(),
                            time,
                            affordance._configuration.Frequency
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