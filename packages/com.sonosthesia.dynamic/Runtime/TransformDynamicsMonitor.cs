using UnityEngine;

namespace Sonosthesia.Dynamic
{
    public class TransformDynamicsMonitor : MonoBehaviour
    {
        [SerializeField] [Range(0, 3)] private int _order = 1;

        [SerializeField] private bool _local = true;

        [SerializeField] private bool _fixedUpdate = true;

        private TransformDynamics.Data? _current;
        private TransformDynamics.Data? _velocity;
        private TransformDynamics.Data? _acceleration;
        private TransformDynamics.Data? _jerk;

        public TransformDynamics.Data Velocity => _velocity ?? default;
        public TransformDynamics.Data Acceleration => _acceleration ?? default;
        public TransformDynamics.Data Jerk => _jerk ?? default;

        public TransformDynamics Dynamics => new TransformDynamics(Velocity, Acceleration, Jerk);

        public TransformDynamics.Data Select(TransformDynamics.Order order)
        {
            return order switch
            {
                TransformDynamics.Order.Velocity => Velocity,
                TransformDynamics.Order.Acceleration => Acceleration,
                TransformDynamics.Order.Jerk => Jerk,
                _ => default
            };
        }
        
        protected virtual void OnEnable()
        {
            _current = null;
            _velocity = null;
            _acceleration = null;
            _jerk = null;
        }

        protected virtual void Update()
        {
            if (!_fixedUpdate)
            {
                Compute(Time.deltaTime);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_fixedUpdate)
            {
                Compute(Time.fixedDeltaTime);
            }
        }
        
        private void Compute(float deltaTime)
        {
            Transform t = transform;
            Vector3 position = _local ? t.localPosition : t.position;
            Vector3 rotation = _local ? t.localRotation.eulerAngles : t.rotation.eulerAngles;
            TransformDynamics.Data updatedCurrent = new TransformDynamics.Data(position, rotation);
            TransformDynamics.Data? updatedVelocity = default;
            TransformDynamics.Data? updatedAcceleration = default;
            TransformDynamics.Data? updatedJerk = default;

            if (_order > 0 && _current.HasValue)
            {
                updatedVelocity = TransformDynamics.Data.Differential(_current.Value, updatedCurrent, deltaTime);
                //Debug.Log($"{this} updated velocity {updatedVelocity.Value}");
            }
            
            if (_order > 1 && _velocity.HasValue && updatedVelocity.HasValue)
            {
                updatedAcceleration = TransformDynamics.Data.Differential(_velocity.Value, updatedVelocity.Value, deltaTime);
            }
            
            if (_order > 2 && _acceleration.HasValue && updatedAcceleration.HasValue)
            {
                updatedJerk = TransformDynamics.Data.Differential(_acceleration.Value, updatedAcceleration.Value, deltaTime);
            }

            _current = updatedCurrent;
            _velocity = updatedVelocity;
            _acceleration = updatedAcceleration;
            _jerk = updatedJerk;
        }
    }
}