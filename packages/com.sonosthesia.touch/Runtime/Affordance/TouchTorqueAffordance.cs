using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTorqueAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private Rigidbody _target;

        [SerializeField] private TouchTorqueConfiguration _configuration;

        private void Apply(Vector3 torque)
        {
            if (_configuration.Relative)
            {
                _target.AddRelativeTorque(torque, _configuration.ForceMode);
            }
            else
            {
                _target.AddTorque(torque);
            }
        }
        
        private class Controller : AffordanceController<TouchEvent, TouchTorqueAffordance>, IDisposable
        {
            private IDynamicExtractorSession<TouchEvent, Vector3> _torqueSession;
            private IInteractiveEnvelopeSession<TouchEvent> _intensitySession;
            
            private Vector3 _torque;
            private float _intensity;
            
            private IDisposable _fixedUpdateSubscription;
            private IDisposable _updateSubscription;
            
            public Controller(Guid eventId, TouchTorqueAffordance affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);
                
                Debug.Assert(_fixedUpdateSubscription == null);
                Debug.Assert(_updateSubscription == null);

                TouchTorqueConfiguration configuration = Affordance._configuration;

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

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                if (Affordance._configuration.Track)
                {
                    _torqueSession.Update(e, out _torque);   
                }
                _intensitySession.Update(e);
            }

            protected override void Teardown(TouchEvent e)
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

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} missing configuration");
            }
            return _target && _configuration ? new Controller(id, this) : null;
        }
    }
}