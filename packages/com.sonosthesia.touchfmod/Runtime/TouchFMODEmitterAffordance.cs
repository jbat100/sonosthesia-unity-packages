using System;
using FMODUnity;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using UniRx;
using UnityEngine;

namespace Sonosthesia.TouchFMOD
{
    public class TouchFMODEmitterAffordance : AbstractAffordance<TouchEvent>
    {
        // these parameters are agreed by convention 
        private static class Parameters
        {
            public const string VOLUME = "Volume";
            public const string EXCITATION = "Excitation";
            public const string BODY = "Body";
        }
        
        [SerializeField] private TouchFMODEmitterConfiguration _configuration;
        
        private class Controller : AffordanceController<TouchEvent, TouchFMODEmitterAffordance>, IDisposable
        {
            private ITouchEnvelopeSession _volumeSession;
            private ITouchEnvelopeSession _excitationSession;
            private ITouchEnvelopeSession _bodySession;
            private IDynamicTrackingSession _positionTrackingSession;
            private IDisposable _updateSubscription;
            private StudioEventEmitter _emitter;
            
            public Controller(Guid eventId, TouchFMODEmitterAffordance affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchFMODEmitterAffordance affordance = Affordance;
                TouchFMODEmitterConfiguration configuration = affordance._configuration;

                _positionTrackingSession = DynamicTrackingSessionUtil.CreateSession(
                    configuration.PositionTracking,
                    e.TouchData.Actor.DynamicsMonitor);
                
                _volumeSession = configuration.Volume.SetupSession(e);
                _excitationSession = configuration.Excitation.SetupSession(e);
                _bodySession = configuration.Body.SetupSession(e);

                _emitter = Instantiate(configuration.EmitterPrefab, affordance.transform);
                _emitter.transform.position = _positionTrackingSession.Update(0f);
                
                _updateSubscription = Observable.EveryUpdate()
                    .StartWith(0) // ensure that it fires immediately
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        _emitter.transform.position = _positionTrackingSession.Update(Time.deltaTime);
                        _emitter.SetParameter(Parameters.VOLUME, _volumeSession.Update());
                        _emitter.SetParameter(Parameters.EXCITATION, _excitationSession.Update());
                        _emitter.SetParameter(Parameters.BODY, _bodySession.Update());
                    }, err => Dispose(), Dispose);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                _volumeSession.UpdateTouch(e);
                _excitationSession.UpdateTouch(e);
                _bodySession.UpdateTouch(e);
            }
            
            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchFMODEmitterAffordance affordance = Affordance;

                _volumeSession.EndTouch(e, out float volumeRelease);
                _excitationSession.EndTouch(e, out float excitationRelease);
                _bodySession.EndTouch(e, out float bodySession);

                float duration = Mathf.Max(volumeRelease, excitationRelease, bodySession);

                Debug.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }
            
            public void Dispose()
            {
                Debug.LogWarning($"{this} Dispose");
                _updateSubscription?.Dispose();
                Destroy(_emitter.gameObject);
            }
        }
        
        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
    }
}