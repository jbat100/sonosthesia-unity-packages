using UnityEngine;

namespace Sonosthesia.Dynamic
{
    public class ProportionalDerivativeFollower : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Header("Position")] 
        
        [SerializeField] private bool _followPosition = true;
        [SerializeField] private float _followForce = 200f;
        [SerializeField] private float _damping = 20f;
        [SerializeField] private float _maxSpeed = 200f;

        [Header("Rotation")] 
        
        [SerializeField] private bool _followRotation = false;
        [SerializeField] private float _rotationSpeed = 1000f;
        [SerializeField] private float _maxTorque = 100f;
        [SerializeField] private float _rotationDamping = 80f;
        
        private Rigidbody _rb;

        protected virtual void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        protected virtual void FixedUpdate()
        {
            if (_followPosition)
            {
                Vector3 directionToTarget = _target.position - transform.position;
                Vector3 proportionalForce = directionToTarget * _followForce;
                Vector3 velocityDamping = -_rb.velocity * _damping;
                Vector3 totalForce = proportionalForce + velocityDamping;
                _rb.AddForce(totalForce);
                if (_rb.velocity.magnitude > _maxSpeed)
                {
                    _rb.velocity = _rb.velocity.normalized * _maxSpeed;
                }   
            }

            if (_followRotation)
            {
                Quaternion targetRotation = _target.rotation;
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
        }
    }
}