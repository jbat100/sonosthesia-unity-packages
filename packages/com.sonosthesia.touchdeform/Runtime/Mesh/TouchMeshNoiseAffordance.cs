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

        [SerializeField] private CompoundNoiseMeshController _controller;
        
        private class Controller : AffordanceController<TouchEvent, TouchMeshNoiseAffordance>, IDisposable
        {
            private ITouchEnvelopeSession _displacementSession;
            private ITouchEnvelopeSession _radiusSession;
            private ITouchEnvelopeSession _speedSession;

            private float3 _center;
            private IDisposable _updateSubscription;
            
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

                _updateSubscription = Observable.EveryUpdate()
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
                        affordance._controller.Register(EventId, info);
                    }, err => Dispose(), Dispose);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                _displacementSession.UpdateTouch(e);
                _radiusSession.UpdateTouch(e);
                _speedSession.UpdateTouch(e);
            }
            
            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchMeshNoiseAffordance affordance = Affordance;

                _displacementSession.EndTouch(e, out float displacementRelease);
                _radiusSession.EndTouch(e, out float radiusRelease);
                _speedSession.EndTouch(e, out float speedRelease);

                float duration = Mathf.Max(displacementRelease, radiusRelease, speedRelease);

                Debug.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }
            
            public void Dispose()
            {
                Debug.LogWarning($"{this} Dispose");
                _updateSubscription?.Dispose();
                Affordance._controller.Unregister(EventId);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
        
    }
}