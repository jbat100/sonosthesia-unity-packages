using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DynamicsTriggerFloatGenerator : StatefulTriggerValueGenerator<DynamicsTriggerFloatGenerator.State, float>
    {
        public class State
        {
            public TransformDynamicsMonitor Monitor { get; set; }
        }
        
        [SerializeField] private TransformDynamics.Order _order;

        [SerializeField] private TransformDynamics.Domain _domain;

        [SerializeField] private Vector3Selector _selector;

        [SerializeField] private bool _abs;

        protected override bool ProcessTriggerEnter(Collider other, State state, out float value)
        {
            TransformDynamicsMonitor monitor = other.GetComponentInParent<TransformDynamicsMonitor>();
            if (monitor)
            {
                state.Monitor = monitor;
                value = Extract(monitor);
                return true;
            }
            value = default;
            return false;
        }

        protected override bool ProcessTriggerStay(Collider other, State state, float initial, float previous, out float value)
        {
            value = Extract(state.Monitor);
            return true;
        }

        protected virtual float Extract(TransformDynamicsMonitor monitor)
        {
            Vector3 v = monitor.Select(_order).Select(_domain);
            if (_abs)
            {
                v = v.Abs();
            }
            return v.Select(_selector);
        }
    }
}