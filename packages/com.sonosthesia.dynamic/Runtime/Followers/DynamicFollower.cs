using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Dynamic
{
    [RequireComponent(typeof(Rigidbody))]
    public class DynamicFollower : Follower
    {
        public enum FollowStrategy
        {
            None,
            Move,
            Force
        }

        [Header("Position")] 
        
        [SerializeField] private FollowStrategy _followPosition = FollowStrategy.Force;
        [SerializeField] private float _followForce = 200f;
        [SerializeField] private float _damping = 20f;
        [SerializeField] private float _maxSpeed = 200f;

        [Header("Rotation")] 
        
        [SerializeField] private FollowStrategy _followRotation = FollowStrategy.Move;
        [SerializeField] private float _rotationSpeed = 1000f;
        [SerializeField] private float _maxTorque = 100f;
        [SerializeField] private float _rotationDamping = 80f;
        
        private Rigidbody _rb;
        private TransformDynamicsMonitor _monitor;

        protected virtual void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();
            _monitor = GetComponent<TransformDynamicsMonitor>();
            
            if (!(Target && _rb))
            {
                Debug.LogError($"{this} requires {nameof(Rigidbody)} and {nameof(Target)}");
                enabled = false;
            }
        }

        protected virtual void FixedUpdate()
        {
            switch (_followPosition)
            {
                case FollowStrategy.Move:
                    _rb.MovePosition(Target.position);
                    break;
                case FollowStrategy.Force:
                {
                    Vector3 directionToTarget = Target.position - transform.position;
                    Vector3 proportionalForce = directionToTarget * _followForce;
                    Vector3 velocityDamping = -_rb.GetLinearVelocity() * _damping;
                    Vector3 totalForce = proportionalForce + velocityDamping;
                    _rb.AddForce(totalForce);
                    if (_rb.GetLinearVelocity().magnitude > _maxSpeed)
                    {
                        _rb.SetLinearVelocity(_rb.GetLinearVelocity().normalized * _maxSpeed);
                    }   
                }
                    break;
            }

            switch (_followRotation)
            {
                case FollowStrategy.Move:
                    _rb.MoveRotation(Target.rotation);
                    break;
                case FollowStrategy.Force:
                {
                    // TODO : fix, this does weird things, prefer Move for now
                    Quaternion targetRotation = Target.rotation;
                    Quaternion currentRotation = transform.rotation;
                    Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
                    rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
                    if (angleInDegrees > 180)
                    {
                        angleInDegrees -= 360;
                    }
                    Vector3 proportionalTorque = rotationAxis * (angleInDegrees * Mathf.Deg2Rad * _rotationSpeed);
                    Vector3 angularVelocityDamping = -_rb.angularVelocity * _rotationDamping;
                    Vector3 totalTorque = proportionalTorque + angularVelocityDamping;
                    _rb.AddTorque(Vector3.ClampMagnitude(totalTorque, _maxTorque));    
                }
                    break;
            }
        }

        public override void Align()
        {
            if (!_rb)
            {
                Debug.LogError($"{this} expected {nameof(Rigidbody)}");
                return;
            }
            
            Vector3 targetPosition = Target.position;
            Quaternion targetRotation = Target.rotation;

            Transform t = transform;
            t.position = targetPosition;
            t.rotation = targetRotation;
            _rb.MovePosition(targetPosition);
            _rb.MoveRotation(targetRotation);

            if (_monitor)
            {
                _monitor.Clear();
            }
        }
    }
}