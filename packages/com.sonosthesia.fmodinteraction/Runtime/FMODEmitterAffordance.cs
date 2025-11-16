using System;
using FMODUnity;
using Sonosthesia.FMOD;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.FMODInteraction
{
    public class FMODEmitterAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct, IInteractionEvent
    {
        // these parameters are agreed by convention 
        private static class Parameters
        {
            public const string VOLUME = "Volume";
            public const string EXCITATION = "Excitation";
            public const string BODY = "Body";
        }
        
        [SerializeField] private InterfaceReference<IFMODEmitterConfiguration<TEvent>> _configuration;

        private IPrefabSelectorSession<StudioEventEmitter> _selectorSession;

        private StudioEventEmitter NextStudioEventEmitter()
        {
            _selectorSession ??= _configuration.Value.Emitter.MakeSession();
            return _selectorSession.Next();
        }

        private class Controller : AffordanceController<TEvent, FMODEmitterAffordance<TEvent>>, IDisposable
        {
            private class ParameterSession : IDisposable
            {
                private string name;
                private IInteractiveEnvelopeSession<TEvent> envelope;
                private bool valid;
                private StudioEventEmitter emitter;
                
                public static ParameterSession Setup(StudioEventEmitter emitter, IInteractiveEnvelopeSettings<TEvent> settings,
                    string name, TEvent e)
                {
                    return new ParameterSession
                    {
                        name = name,
                        emitter = emitter,
                        envelope = settings.StartSession(e),
                        valid = emitter.HasParameter(name)
                    };
                }

                public void UpdateTouch(TEvent e) => envelope.Update(e);

                public void EndTouch(TEvent e, out float release) => envelope.End(e, out release);

                public float Update()
                {
                    if (valid)
                    {
                        float value = envelope.Evaluate();
                        emitter.SetParameter(name, value);
                        return value;
                    }

                    return float.NaN;
                }

                public void Dispose()
                {
                    envelope?.Dispose();
                }
            }
            
            private ParameterSession _volumeSession;
            private ParameterSession _excitationSession;
            private ParameterSession _bodySession;
            
            private IDynamicTrackingSession _positionTrackingSession;
            private IDisposable _updateSubscription;
            private StudioEventEmitter _emitter;
            
            public Controller(Guid eventId, FMODEmitterAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                FMODEmitterAffordance<TEvent> affordance = Affordance;
                IFMODEmitterConfiguration<TEvent> configuration = affordance._configuration.Value;

                _positionTrackingSession = DynamicTrackingSessionUtil.CreateSession(
                    configuration.PositionTracking,
                    e.Actor.DynamicsMonitor);
                
                _emitter = Instantiate(Affordance.NextStudioEventEmitter(), affordance.transform);
                _emitter.transform.position = _positionTrackingSession.Update(0f);
                
                _volumeSession = ParameterSession.Setup(_emitter, configuration.Volume, Parameters.VOLUME, e);
                _excitationSession = ParameterSession.Setup(_emitter, configuration.Excitation, Parameters.EXCITATION, e);
                _bodySession = ParameterSession.Setup(_emitter, configuration.Body, Parameters.BODY, e);
                
                void UpdateParameters()
                {
                    float volume = _volumeSession.Update();
                    float excitation = _excitationSession.Update();
                    float body = _bodySession.Update();
                    affordance.LogVerbose($"{affordance} update FMOD parameters : " +
                                          $"{nameof(volume)} {volume} {nameof(excitation)} {excitation} {nameof(body)} {body}");
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

            protected override void Update(TEvent e)
            {
                base.Update(e);
                
                _volumeSession.UpdateTouch(e);
                _excitationSession.UpdateTouch(e);
                _bodySession.UpdateTouch(e);
            }
            
            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                
                FMODEmitterAffordance<TEvent> affordance = Affordance;

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
                _volumeSession.Dispose();
                _excitationSession.Dispose();
                _bodySession.Dispose();
                Destroy(_emitter.gameObject);
            }
        }
        
        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (_configuration.Value == null)
            {
                this.LogError($"{this} missing configuration");
            }
            return _configuration?.Value != null ? new Controller(id, this) : null;
        }

        protected virtual void OnValidate()
        {
            _selectorSession = null;
        }
    }
}