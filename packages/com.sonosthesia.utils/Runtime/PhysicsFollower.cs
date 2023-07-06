using UnityEngine;

namespace Sonosthesia.Utils
{
    public class PhysicsFollower : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private bool _position = true;
        [SerializeField] private bool _rotation = true;
        
        protected virtual void Awake()
        {
            if (!_rigidbody)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }
        
        protected virtual void FixedUpdate()
        {
            if (_position)
            {
                _rigidbody.MovePosition(_target.transform.position);    
            }
            if (_rotation)
            {
                _rigidbody.MoveRotation(_target.transform.rotation);    
            }
        }
    }
}
