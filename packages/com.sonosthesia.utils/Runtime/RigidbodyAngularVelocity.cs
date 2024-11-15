using UnityEngine;

namespace Sonosthesia.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyAngularVelocity : MonoBehaviour
    {
        [SerializeField] private float _maxAngularVelocity;
        [SerializeField] private Vector3 _angularVelocity;
        
        private Rigidbody _rigidbody;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected void FixedUpdate()
        {
            _rigidbody.maxAngularVelocity = _maxAngularVelocity;
            _angularVelocity = _rigidbody.angularVelocity;
        }
    }
}