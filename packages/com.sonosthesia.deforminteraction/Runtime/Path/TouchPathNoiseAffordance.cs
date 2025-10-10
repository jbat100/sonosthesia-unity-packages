using System;
using Unity.Mathematics;
using UnityEngine;
using UniRx;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;

namespace Sonosthesia.DeformInteraction
{
    public class TouchPathNoiseAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private TouchPathNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoisePathProcessor _processor;

        private class Controller : AffordanceController<TouchEvent, TouchPathNoiseAffordance>, IDisposable
        {
            // we can't use the Update(TouchEvent) callback to run the sessions because:
            // - it is not necessarily called on each frame
            // - it is not called beyond teardown
            // feels like this is common enough that it justifies an inbuilt mechanism 

            private IInteractiveEnvelopeSession<TouchEvent> _displacementSession;
            private IInteractiveEnvelopeSession<TouchEvent> _radiusSession;
            private IInteractiveEnvelopeSession<TouchEvent> _frequencySession;
            private IInteractiveEnvelopeSession<TouchEvent> _speedSession;

            private float3 _center;
            
            private IDisposable _updateSubscription;

            public Controller(Guid eventId, TouchPathNoiseAffordance affordance) : base(eventId, affordance)
            {
            }

            private float3 ExtractPosition(TouchEvent e)
            {
                return e.touchData.Actor.transform.position;
            }
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchPathNoiseAffordance affordance = Affordance;
                TouchPathNoiseConfiguration configuration = affordance._configuration;

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
                        time += Time.deltaTime * _speedSession.Update();
                        CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                            configuration.NoiseType,
                            _displacementSession.Update(),
                            configuration.FalloffType,
                            _center,
                            _radiusSession.Update(),
                            time,
                            float3.zero,
                            _frequencySession.Update()
                        );
                        affordance._processor.Register(EventId, info);
                    }, err => Dispose(), Dispose);
                    
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                if (Affordance._configuration.TrackPosition)
                {
                    _center = ExtractPosition(e);
                }
                
                _displacementSession.Update(e);
                _radiusSession.Update(e);
                _frequencySession.Update(e);
                _speedSession.Update(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchPathNoiseAffordance affordance = Affordance;

                _displacementSession.End(e, out float displacementRelease);
                _radiusSession.End(e, out float radiusRelease);
                _frequencySession.End(e, out float frequencyRelease);
                _speedSession.End(e, out float speedRelease);

                float duration = Mathf.Max(displacementRelease, radiusRelease, frequencyRelease, speedRelease);

                Debug.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }

            public void Dispose()
            {
                Debug.LogWarning($"{this} Dispose");
                _updateSubscription?.Dispose();
                Affordance._processor.Unregister(EventId);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
    }
}