using System;
using FMODUnity;
using Sonosthesia.FMOD;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.FMODInteraction
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

        private IPrefabSelectorSession<StudioEventEmitter> _selectorSession;

        private StudioEventEmitter NextStudioEventEmitter()
        {
            _selectorSession ??= _configuration.Emitter.MakeSession();
            return _selectorSession.Next();
        }

        private class Controller : AffordanceController<TouchEvent, TouchFMODEmitterAffordance>, IDisposable
        {
            private struct ParameterSession
            {
                private string name;
                private ITouchEnvelopeSession envelope;
                private bool valid;
                private StudioEventEmitter emitter;
                
                public static ParameterSession Setup(StudioEventEmitter emitter, TouchEnvelopeSettings settings,
                    string name, TouchEvent e)
                {
                    return new ParameterSession
                    {
                        name = name,
                        emitter = emitter,
                        envelope = settings.SetupSession(e),
                        valid = emitter.HasParameter(name)
                    };
                }

                public void UpdateTouch(TouchEvent e) => envelope.UpdateTouch(e);

                public void EndTouch(TouchEvent e, out float release) => envelope.EndTouch(e, out release);

                public void Update()
                {
                    if (valid)
                    {
                        emitter.SetParameter(name, envelope.Update());    
                    }
                }
            }
            
            private ParameterSession _volumeSession;
            private ParameterSession _excitationSession;
            private ParameterSession _bodySession;
            
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
                    e.touchData.Actor.DynamicsMonitor);
                
                _emitter = Instantiate(Affordance.NextStudioEventEmitter(), affordance.transform);
                _emitter.transform.position = _positionTrackingSession.Update(0f);
                
                _volumeSession = ParameterSession.Setup(_emitter, configuration.Volume, Parameters.VOLUME, e);
                _excitationSession = ParameterSession.Setup(_emitter, configuration.Excitation, Parameters.EXCITATION, e);
                _bodySession = ParameterSession.Setup(_emitter, configuration.Body, Parameters.BODY, e);
                
                void UpdateParameters()
                {
                    _volumeSession.Update();
                    _excitationSession.Update();
                    _bodySession.Update();
                }
                
                UpdateParameters();
                
                _updateSubscription = Observable.EveryUpdate()
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ =>
                    {
                        _emitter.transform.position = _positionTrackingSession.Update(Time.deltaTime);
                        UpdateParameters();
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

                Affordance.LogWarning($"{this} {nameof(Teardown)} Dispose in {duration} seconds");
                
                Observable.Timer(TimeSpan.FromSeconds(duration))
                    .TakeUntilDisable(affordance)
                    .Subscribe(_ => {}, Dispose);
            }
            
            public void Dispose()
            {
                Affordance.LogWarning($"{this} Dispose");
                _updateSubscription?.Dispose();
                Destroy(_emitter.gameObject);
            }
        }
        
        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }

        protected virtual void OnValidate()
        {
            _selectorSession = null;
        }
    }
}