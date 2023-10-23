using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DynamicsTriggerVectorGenerator : StatefulTriggerValueGenerator<DynamicsTriggerVectorGenerator.State, Vector3>
    {
        public class State
        {
            public TransformDynamicsMonitor Monitor { get; set; }
        }
        
        [SerializeField] private TransformDynamics.Order _order;

        [SerializeField] private TransformDynamics.Domain _domain;

        protected override bool ProcessTriggerEnter(Collider other, State state, out Vector3 value)
        {
            TransformDynamicsMonitor monitor = other.GetComponentInParent<TransformDynamicsMonitor>();
            if (monitor)
            {
                state.Monitor = monitor;
                value = monitor.Select(_order).Select(_domain);
                return true;
            }
            value = default;
            return false;;
        }

        protected override bool ProcessTriggerStay(Collider other, State state, Vector3 initial, Vector3 previous, out Vector3 value)
        {
            value = state.Monitor.Select(_order).Select(_domain);
            return true;
        }
    }
}