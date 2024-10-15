using System;
using Unity.Mathematics;
using UnityEngine;
using UniRx;
using Sonosthesia.Deform;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;

namespace Sonosthesia.TouchDeform
{
    public class TrackedTouchPathNoiseAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private TrackedTouchPathNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoisePathProcessor _processor;

        private class Controller : AffordanceController<TouchEvent, TrackedTouchPathNoiseAffordance>, IDisposable
        {
            // we can't use the Update(TouchEvent) callback to run the sessions because:
            // - it is not necessarily called on each frame
            // - it is not called beyond teardown
            // feels like this is common enough that it justifies an inbuilt mechanism 

            private TrackedTouchEnvelopeSession _displacementSession;
            private TrackedTouchEnvelopeSession _radiusSession;
            private TrackedTouchEnvelopeSession _frequencySession;
            private TrackedTouchEnvelopeSession _speedSession;

            private float3 _center;
            
            private IDisposable _updateSubscription;

            public Controller(Guid eventId, TrackedTouchPathNoiseAffordance affordance) : base(eventId, affordance)
            {
            }

            private float3 ExtractPosition(TouchEvent e)
            {
                return e.TouchData.Actor.transform.position;
            }
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TrackedTouchPathNoiseAffordance affordance = Affordance;
                TrackedTouchPathNoiseConfiguration configuration = affordance._configuration;

                _center = ExtractPosition(e);
                
                _displacementSession = configuration.Displacement.SetupSession(e);
                _frequencySession = configuration.Frequency.SetupSession(e);
                _speedSession = configuration.Speed.SetupSession(e);
                _radiusSession = configuration.Radius.SetupSession(e);

                float time = 0f;
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
                
                TrackedTouchPathNoiseConfiguration configuration = Affordance._configuration;

                if (configuration.TrackPosition)
                {
                    _center = ExtractPosition(e);
                }
                
                _displacementSession.UpdateTouch(e);
                _radiusSession.UpdateTouch(e);
                _frequencySession.UpdateTouch(e);
                _speedSession.UpdateTouch(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TrackedTouchPathNoiseAffordance affordance = Affordance;

                _displacementSession.EndTouch(e, out float displacementDuration);
                _radiusSession.EndTouch(e, out float radiusDuration);
                _frequencySession.EndTouch(e, out float frequencyDuration);
                _speedSession.EndTouch(e, out float speedDuration);

                float duration = Mathf.Max(displacementDuration, radiusDuration, frequencyDuration, speedDuration);

                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => Dispose(), Dispose);
            }

            public void Dispose()
            {
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