using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TorqueAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private Rigidbody _target;

        [SerializeField] private InterfaceReference<ITorqueConfiguration<TEvent>> _configuration;

        private void Apply(Vector3 torque)
        {
            if (_configuration.Value is { Relative: true })
            {
                _target.AddRelativeTorque(torque, _configuration.Value.ForceMode);
            }
            else
            {
                _target.AddTorque(torque);
            }
        }
        
        private class Controller : AffordanceController<TEvent, TorqueAffordance<TEvent>>, IDisposable
        {
            private IDynamicExtractorSession<TEvent, Vector3> _torqueSession;
            private IInteractiveEnvelopeSession<TEvent> _intensitySession;
            
            private Vector3 _torque;
            private float _intensity;
            
            private IDisposable _fixedUpdateSubscription;
            private IDisposable _updateSubscription;
            
            public Controller(Guid eventId, TorqueAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TEvent e)
            {
                base.Setup(e);
                
                Debug.Assert(_fixedUpdateSubscription == null);
                Debug.Assert(_updateSubscription == null);

                ITorqueConfiguration<TEvent> configuration = Affordance._configuration.Value;

                _torqueSession = configuration.Torque.SetupSession(e, out _torque);
                _intensitySession = configuration.Intensity.StartSession(e);

                _updateSubscription = Observable.EveryUpdate().StartWith(0).Subscribe(_ =>
                {
                    _intensity = _intensitySession.Update();
                });
                
                _fixedUpdateSubscription = Observable.EveryFixedUpdate().Subscribe(_ =>
                {
                    Affordance.Apply(_torque * _intensity);
                });
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                if (Affordance._configuration.Value.Track)
                {
                    _torqueSession.Update(e, out _torque);   
                }
                _intensitySession.Update(e);
            }

            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                _intensitySession.End(e, out float _release);
                Observable.Timer(TimeSpan.FromSeconds(_release)).Subscribe(_ => Dispose());
            }

            public void Dispose()
            {
                _fixedUpdateSubscription?.Dispose();
                _fixedUpdateSubscription = null;
                
                _updateSubscription?.Dispose();
                _updateSubscription = null;
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (_configuration.Value == null)
            {
                this.LogError($"{this} missing configuration");
            }
            return _target && _configuration != null ? new Controller(id, this) : null;
        }
    }
}