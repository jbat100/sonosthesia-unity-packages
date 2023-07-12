using UnityEngine;

namespace Sonosthesia.Utils
{
    public class DynamicTransformMonitor : MonoBehaviour
    {
        [SerializeField] [Range(0, 3)] private int _order = 1;

        [SerializeField] private bool _local = true;

        private DynamicTransform.Data? _current;
        private DynamicTransform.Data? _velocity;
        private DynamicTransform.Data? _acceleration;
        private DynamicTransform.Data? _jerk;

        public DynamicTransform.Data Velocity => _velocity ?? default;
        public DynamicTransform.Data Acceleration => _acceleration ?? default;
        public DynamicTransform.Data Jerk => _jerk ?? default;

        public DynamicTransform Data => new DynamicTransform(Velocity, Acceleration, Jerk);

        protected virtual void OnEnable()
        {
            _current = null;
            _velocity = null;
            _acceleration = null;
            _jerk = null;
        }

        protected virtual void Update()
        {
            Transform t = transform;
            Vector3 position = _local ? t.localPosition : t.position;
            Vector3 rotation = _local ? t.localRotation.eulerAngles : t.rotation.eulerAngles;
            DynamicTransform.Data updatedCurrent = new DynamicTransform.Data(position, rotation);
            DynamicTransform.Data? updatedVelocity = default;
            DynamicTransform.Data? updatedAcceleration = default;
            DynamicTransform.Data? updatedJerk = default;

            if (_order > 0 && _current != null)
            {
                updatedVelocity = DynamicTransform.Data.Differential(_current.Value, updatedCurrent, Time.deltaTime);
            }
            
            if (_order > 1 && _velocity.HasValue && updatedVelocity.HasValue)
            {
                updatedAcceleration = DynamicTransform.Data.Differential(_velocity.Value, updatedVelocity.Value, Time.deltaTime);
            }
            
            if (_order > 2 && _acceleration.HasValue && updatedAcceleration.HasValue)
            {
                updatedJerk = DynamicTransform.Data.Differential(_acceleration.Value, updatedAcceleration.Value, Time.deltaTime);
            }

            _current = updatedCurrent;
            _velocity = updatedVelocity;
            _acceleration = updatedAcceleration;
            _jerk = updatedJerk;

        }
        
    }
}