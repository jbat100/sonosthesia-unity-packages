using UnityEngine;

namespace Sonosthesia.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class VelocityTracking : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] bool _trackPosition = true;

        [SerializeField, Range(0, 1)] private float _velocityDamping = 1f;
        
        [SerializeField] private float _velocityScale = 1f;

        [SerializeField] bool _trackRotation = true;
        
        private Rigidbody _rigidbody;
        

        protected virtual void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        protected virtual void FixedUpdate()
        {
            if (_trackPosition)
            {    
                _rigidbody.velocity *= (1f - _velocityDamping);
                Vector3 positionDelta = _target.position - transform.position;
                Vector3 velocity = positionDelta / Time.deltaTime;
                _rigidbody.velocity += (velocity * _velocityScale);
            }

            if (_trackRotation)
            {
                _rigidbody.rotation = _target.rotation;
            }
        }

    }
}