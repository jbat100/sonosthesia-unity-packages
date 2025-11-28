using System;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
 public class PathNoiseAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private InterfaceReference<IPathNoiseConfiguration<TEvent>> _configuration;

        [SerializeField] private CompoundNoisePathProcessor _processor;

        private class Controller : AffordanceController<TEvent, PathNoiseAffordance<TEvent>>, IDisposable
        {
            // we can't use the Update(TouchEvent) callback to run the sessions because:
            // - it is not necessarily called on each frame
            // - it is not called beyond teardown
            // feels like this is common enough that it justifies an inbuilt mechanism 

            private IInteractiveEnvelopeSession<TEvent> _displacementSession;
            private IInteractiveEnvelopeSession<TEvent> _radiusSession;
            private IInteractiveEnvelopeSession<TEvent> _frequencySession;
            private IInteractiveEnvelopeSession<TEvent> _speedSession;

            private float3 _center;
            
            private IDisposable _updateSubscription;

            public Controller(Guid eventId, PathNoiseAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }

            private float3 ExtractPosition(TEvent e)
            {
                return e.Actor.Transform.position;
            }
            
            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                PathNoiseAffordance<TEvent> affordance = Affordance;
                IPathNoiseConfiguration<TEvent> configuration = affordance._configuration.Value;

                _center = ExtractPosition(e);
                
                _displacementSession = configuration.Displacement.StartSession(e);
                _frequencySession = configuration.Frequency.StartSession(e);
                _speedSession = configuration.Speed.StartSession(e);
                _radiusSession = configuration.Radius.StartSession(e);

                // don't start time from 0 or we get always the same noise effect, the choice of Time.time 
                // is arbitrary, it could be a random number
                
                float time = Time.time;
                
                _updateSubscription = Observable.EveryUpdate()
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        time += Time.deltaTime * _speedSession.Evaluate();
                        CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                            configuration.NoiseType,
                            _displacementSession.Evaluate(),
                            configuration.FalloffType,
                            _center,
                            _radiusSession.Evaluate(),
                            time,
                            float3.zero,
                            _frequencySession.Evaluate()
                        );
                        affordance._processor.Register(EventId, info);
                    }, err => Dispose(), Dispose);
                    
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                
                if (Affordance._configuration.Value.TrackPosition)
                {
                    _center = ExtractPosition(e);
                }
                
                _displacementSession.Update(e);
                _radiusSession.Update(e);
                _frequencySession.Update(e);
                _speedSession.Update(e);
            }

            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                
                PathNoiseAffordance<TEvent> affordance = Affordance;

                _displacementSession.End(e, out float displacementRelease);
                _radiusSession.End(e, out float radiusRelease);
                _frequencySession.End(e, out float frequencyRelease);
                _speedSession.End(e, out float speedRelease);

                float duration = Mathf.Max(displacementRelease, radiusRelease, frequencyRelease, speedRelease);

                Affordance.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }

            public void Dispose()
            {
                Affordance.LogWarning($"{this} Dispose");
                _updateSubscription?.Dispose();
                Affordance._processor.Unregister(EventId);
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} has no configuration.");
            }
            return _configuration?.Value != null ? new Controller(id, this) : null;
        }
    }
}