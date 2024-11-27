using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Dynamic
{
    public class TransformMonitor : MonoBehaviour
    {
        private Vector3? _ultimatePosition;
        private Vector3? _penultimatePosition;
    
        public Vector3 Velocity { get; private set; }
        public Vector3 Acceleration  { get; private set; }
    
        protected virtual void OnEnable()
        {
            Velocity = Vector3.zero;
            Acceleration = Vector3.zero;
        
            _ultimatePosition = null;
            _penultimatePosition = null;
        }
        
        protected virtual void Update()
        {
            Vector3 position = transform.position;

            if (_ultimatePosition.HasValue)
            {
                Velocity = (position - _ultimatePosition.Value) / Time.deltaTime;
            }
            if (_ultimatePosition.HasValue && _penultimatePosition.HasValue)
            {
                Vector3 ultimateVelocity = (_ultimatePosition.Value - _penultimatePosition.Value) / Time.deltaTime;
                Acceleration = (Velocity - ultimateVelocity) / Time.deltaTime;
            }
        
            _penultimatePosition = _ultimatePosition;
            _ultimatePosition = position;
        }
    }    
}


