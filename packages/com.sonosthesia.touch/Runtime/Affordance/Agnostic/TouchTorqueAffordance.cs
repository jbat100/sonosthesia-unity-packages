using System;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTorqueAffordance : AbstractAffordance<TouchEvent>
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
            private ITouchExtractorSession<Vector3> _torqueSession;
            private Vector3 _torque;
            private IDisposable _fixedUpdateSubscription;
            
            public Controller(Guid eventId, TouchTorqueAffordance affordance) : base(eventId, affordance)
            {
            }
            
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchTorqueConfiguration configuration = Affordance._configuration;

                _torqueSession = configuration.Torque.MakeSession();
                _torqueSession.Setup(e, out _torque);

                _fixedUpdateSubscription = Observable.EveryFixedUpdate().Subscribe(_ => Affordance.Apply(_torque));
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                if (Affordance._configuration.Track)
                {
                    _torqueSession.Update(e, out _torque);   
                }
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                Dispose();
            }

            public void Dispose()
            {
                _fixedUpdateSubscription?.Dispose();
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id) 
            => _target && _configuration ? new Controller(id, this) : null;
    }
}