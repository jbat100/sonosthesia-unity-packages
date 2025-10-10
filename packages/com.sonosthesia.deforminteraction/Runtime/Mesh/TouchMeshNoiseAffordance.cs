using System;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using Sonosthesia.Noise;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    public class TouchMeshNoiseAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private TouchMeshNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoiseMeshController _controller;
        
        private class Controller : AffordanceController<TouchEvent, TouchMeshNoiseAffordance>, IDisposable
        {
            private IInteractiveEnvelopeSession<TouchEvent> _displacementSession;
            private IInteractiveEnvelopeSession<TouchEvent> _radiusSession;
            private IInteractiveEnvelopeSession<TouchEvent> _speedSession;

            private IDynamicTrackingSession _actorTrackingSession;

            private IDisposable _updateSubscription;
            
            public Controller(Guid eventId, TouchMeshNoiseAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchMeshNoiseAffordance affordance = Affordance;
                TouchMeshNoiseConfiguration configuration = affordance._configuration;

                _actorTrackingSession = DynamicTrackingSessionUtil.CreateSession(
                    configuration.ActorTracking,
                    e.touchData.Actor.DynamicsMonitor);
                
                _displacementSession = configuration.Displacement.StartSession(e);
                _radiusSession = configuration.Radius.StartSession(e);
                _speedSession = configuration.Speed.StartSession(e);

                // don't start time from 0 or we get always the same noise effect, the choice of Time.time 
                // is arbitrary, it could be a random number
                
                float time = Time.time;
                
                float3x4 rts = (new SpaceTRS { scale = 1 }).Matrix;

                Vector3 source = e.touchData.Source.transform.position;

                _updateSubscription = Observable.EveryUpdate()
                    // .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        time += Time.deltaTime * _speedSession.Update();
                        Vector3 actor = _actorTrackingSession.Update(Time.deltaTime);
                        Vector3 center = configuration.SpatialFalloff.Center switch
                        {
                            TouchSpatialFalloffCenter.Actor => actor,
                            _ => source
                        };
                        Vector3 handle = center + configuration.SpatialFalloff.Space switch
                        {
                            TouchSpatialFalloffSpace.World => configuration.SpatialFalloff.Offset,
                            TouchSpatialFalloffSpace.Source => e.touchData.Source.transform.TransformDirection(configuration.SpatialFalloff.Offset),
                            TouchSpatialFalloffSpace.Actor => e.touchData.Actor.transform.TransformDirection(configuration.SpatialFalloff.Offset),
                            TouchSpatialFalloffSpace.ActorSource => (actor - source).normalized * configuration.SpatialFalloff.Offset.y,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        SpatialFalloffInfo falloffInfo = new SpatialFalloffInfo(
                            configuration.SpatialFalloff.Active,
                            configuration.SpatialFalloff.Shape,
                            configuration.SpatialFalloff.EaseType,
                            center, handle, _radiusSession.Update());
                        CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                            configuration.CrossFadeType,
                            configuration.NoiseType,
                            _displacementSession.Update(),
                            rts,
                            falloffInfo,
                            time,
                            configuration.Frequency
                        );
                        affordance._controller.Register(EventId, info);
                    }, err => Dispose(), Dispose);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                _displacementSession.Update(e);
                _radiusSession.Update(e);
                _speedSession.Update(e);
            }
            
            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchMeshNoiseAffordance affordance = Affordance;

                _displacementSession.End(e, out float displacementRelease);
                _radiusSession.End(e, out float radiusRelease);
                _speedSession.End(e, out float speedRelease);

                float duration = Mathf.Max(displacementRelease, radiusRelease, speedRelease);

                // Debug.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    // .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }
            
            public void Dispose()
            {
                // Debug.LogWarning($"{this} Dispose");
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