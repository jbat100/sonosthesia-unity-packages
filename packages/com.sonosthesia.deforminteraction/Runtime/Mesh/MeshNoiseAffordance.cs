using System;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Noise;
using Sonosthesia.Utils;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    public class MeshNoiseAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private InterfaceReference<IMeshNoiseConfiguration<TEvent>> _configuration;

        [SerializeField] private CompoundNoiseMeshController _controller;
        
        private class Controller : AffordanceController<TEvent, MeshNoiseAffordance<TEvent>>, IDisposable
        {
            private IInteractiveEnvelopeSession<TEvent> _displacementSession;
            private IInteractiveEnvelopeSession<TEvent> _radiusSession;
            private IInteractiveEnvelopeSession<TEvent> _speedSession;

            private IDynamicTrackingSession _actorTrackingSession;

            private IDisposable _updateSubscription;
            
            public Controller(Guid eventId, MeshNoiseAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                MeshNoiseAffordance<TEvent> affordance = Affordance;
                IMeshNoiseConfiguration<TEvent> configuration = affordance._configuration.Value;

                _actorTrackingSession = DynamicTrackingSessionUtil.CreateSession(
                    configuration.ActorTracking,
                    e.Actor.DynamicsMonitor);
                
                _displacementSession = configuration.Displacement.StartSession(e);
                _radiusSession = configuration.Radius.StartSession(e);
                _speedSession = configuration.Speed.StartSession(e);

                // don't start time from 0 or we get always the same noise effect, the choice of Time.time 
                // is arbitrary, it could be a random number
                
                float time = Time.time;
                
                float3x4 rts = (new SpaceTRS { scale = 1 }).Matrix;

                Vector3 source = e.Source.Transform.position;

                _updateSubscription = Observable.EveryUpdate()
                    // .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        time += Time.deltaTime * _speedSession.Evaluate();
                        Vector3 actor = _actorTrackingSession.Update(Time.deltaTime);
                        Vector3 center = configuration.SpatialFalloff.Center switch
                        {
                            SpatialFalloffCenter.Actor => actor,
                            _ => source
                        };
                        Vector3 handle = center + configuration.SpatialFalloff.Space switch
                        {
                            SpatialFalloffSpace.World => configuration.SpatialFalloff.Offset,
                            SpatialFalloffSpace.Source => e.Source.Transform.TransformDirection(configuration.SpatialFalloff.Offset),
                            SpatialFalloffSpace.Actor => e.Actor.Transform.TransformDirection(configuration.SpatialFalloff.Offset),
                            SpatialFalloffSpace.ActorSource => (actor - source).normalized * configuration.SpatialFalloff.Offset.y,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        SpatialFalloffInfo falloffInfo = new SpatialFalloffInfo(
                            configuration.SpatialFalloff.Active,
                            configuration.SpatialFalloff.Shape,
                            configuration.SpatialFalloff.EaseType,
                            center, handle, _radiusSession.Evaluate());
                        CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                            configuration.CrossFadeType,
                            configuration.NoiseType,
                            _displacementSession.Evaluate(),
                            rts,
                            falloffInfo,
                            time,
                            configuration.Frequency
                        );
                        affordance._controller.Register(EventId, info);
                    }, err => Dispose(), Dispose);
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                
                _displacementSession.Update(e);
                _radiusSession.Update(e);
                _speedSession.Update(e);
            }
            
            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                
                MeshNoiseAffordance<TEvent> affordance = Affordance;

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
                _displacementSession?.Dispose();
                _radiusSession?.Dispose();
                _speedSession?.Dispose();
                Affordance._controller.Unregister(EventId);
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (_configuration?.Value == null)
            {
                this.LogError($"{this} has no configuration.");
            }
            return _configuration?.Value != null ? new Controller(id, this) : null;
        }
    }
}