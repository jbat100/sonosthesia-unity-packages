using System;
using UnityEngine;

namespace Sonosthesia.Dynamic
{
    [RequireComponent(typeof(TransformDynamicsMonitor))]
    public class TransformDynamicsMonitorDebug : MonoBehaviour
    {
        [SerializeField] private TransformDynamics.Order _order;

        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _rotation;
        
        private TransformDynamicsMonitor _monitor;

        protected void Awake()
        {
            _monitor = GetComponent<TransformDynamicsMonitor>();
        }

        protected void Update()
        {
            TransformDynamics.Data data = _monitor.Dynamics.Select(_order);
            _position = data.Position;
            _rotation = data.Rotation;
        }
        
    }
}