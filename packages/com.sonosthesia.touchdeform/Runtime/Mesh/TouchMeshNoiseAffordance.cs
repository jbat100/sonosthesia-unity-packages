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
            private ITouchEnvelopeSession _displacementSession;
            private ITouchEnvelopeSession _radiusSession;
            private ITouchEnvelopeSession _speedSession;

            private float3 _center;
            
            public Controller(Guid eventId, TouchMeshNoiseAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchMeshNoiseAffordance affordance = Affordance;
                TouchMeshNoiseConfiguration configuration = affordance._configuration;

                _center = e.TouchData.Actor.transform.position;

                _displacementSession = configuration.Displacement.SetupSession(e);
                _radiusSession = configuration.Radius.SetupSession(e);
                _speedSession = configuration.Speed.SetupSession(e);
                
                float time = 0f;
                float3x4 rts = (new SpaceTRS { scale = 1 }).Matrix;
                
                void Cleanup()
                {
                    affordance._processor.Unregister(EventId);
                }

                Observable.EveryUpdate()
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        time += Time.deltaTime * _speedSession.Update();
                        CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                            affordance._configuration.CrossFadeType,
                            affordance._configuration.NoiseType,
                            _displacementSession.Update(),
                            rts,
                            affordance._configuration.Falloff,
                            affordance._configuration.FalloffType,
                            _center,
                            _radiusSession.Update(),
                            time,
                            affordance._configuration.Frequency
                        );
                        affordance._processor.Register(EventId, info);
                    }, err => Cleanup(), Cleanup);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                _displacementSession.UpdateTouch(e);
                _radiusSession.UpdateTouch(e);
                _speedSession.UpdateTouch(e);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
        
    }
}