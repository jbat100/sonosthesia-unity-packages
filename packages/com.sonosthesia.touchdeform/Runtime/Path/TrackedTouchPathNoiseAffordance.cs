using System;
using Unity.Mathematics;
using UnityEngine;
using UniRx;
using Sonosthesia.Deform;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using Sonosthesia.Trigger;

namespace Sonosthesia.TouchDeform
{
    public class TrackedTouchPathNoiseAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private TrackedTouchPathNoiseConfiguration _configuration;

        [SerializeField] private CompoundNoisePathProcessor _processor;

        private class Controller : AffordanceController<TouchEvent, TrackedTouchPathNoiseAffordance>, IDisposable
        {
            private TrackedTriggerController _displacementController;

            // we can't use the Update callback because:
            // - it is not necessarily called on each frame
            // - it is not called beyond teardown
            // feels like this is common enough that it justifies an inbuilt mechanism 

            private float3 _center;
            private float _radius;
            private float _time;
            
            private IDisposable _updateSubscription;
            private ITouchExtractorSession<float> _displacementValueSession;
            private ITouchExtractorSession<float> _radiusSession;

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
                
                if (!affordance._configuration)
                {
                    return;
                }
                
                _displacementController = new TrackedTriggerController(AccumulationMode.Max);

                _displacementValueSession = affordance._configuration.DisplacementValueExtractor.MakeSession();
                _displacementValueSession.Setup(e, out float valueScale);
                
                ITouchExtractorSession<float> displacementTimeSession = affordance._configuration.DisplacementTimeExtractor.MakeSession();
                displacementTimeSession.Setup(e, out float timeScale);

                IEnvelope envelope = affordance._configuration.DisplacementEnvelope.Build();

                _displacementController.StartTrigger(EventId, envelope, valueScale, timeScale);

                _center = ExtractPosition(e);

                _radiusSession = affordance._configuration.RadiusExtractor.MakeSession();
                _radiusSession.Setup(e, out _radius);

                float speed = affordance._configuration.Speed;
                
                _updateSubscription = Observable.EveryUpdate()
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        float displacement = _displacementController.Update();
                        _time += Time.deltaTime * speed;
                        CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                            affordance._configuration.NoiseType,
                            displacement,
                            affordance._configuration.FalloffType,
                            _center,
                            _radius,
                            _time,
                            float3.zero,
                            affordance._configuration.Frequency
                        );
                        affordance._processor.Register(EventId, info);
                    }, err =>
                    {
                        affordance._processor.Unregister(EventId);
                        Dispose();
                    }, () =>
                    {
                        affordance._processor.Unregister(EventId);
                        Dispose();
                    });
                    
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                TrackedTouchPathNoiseAffordance affordance = Affordance;

                if (!affordance._configuration)
                {
                    return;
                }
                
                if (affordance._configuration.TrackDisplacement)
                {
                    _displacementValueSession.Update(e, out float valueScale);
                    _displacementController.UpdateTrigger(EventId, valueScale);
                }

                if (affordance._configuration.TrackPosition)
                {
                    _center = ExtractPosition(e);
                }

                if (affordance._configuration.TrackRadius)
                {
                    _radiusSession.Update(e, out _radius);
                }
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TrackedTouchPathNoiseAffordance affordance = Affordance;

                if (!affordance._configuration)
                {
                    return;
                }

                ITouchExtractorSession<float> displacementReleaseSession 
                    = affordance._configuration.DisplacementReleaseExtractor.MakeSession();
                
                displacementReleaseSession.Setup(e, out float release);

                IEnvelope releaseEnvelope = affordance._configuration.DisplacementReleaseEaseType.ReleaseEnvelope(release);

                _displacementController.EndTrigger(EventId, releaseEnvelope);

                Observable.Timer(TimeSpan.FromSeconds(release))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        affordance._processor.Unregister(EventId);
                        Dispose();
                    }, () =>
                    {
                        affordance._processor.Unregister(EventId);
                        Dispose();
                    });
            }

            public void Dispose()
            {
                _displacementController?.Dispose();
                _updateSubscription?.Dispose();
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}