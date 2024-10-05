using Sonosthesia.Dynamic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DynamicsTouchVectorGenerator : StatefulTouchValueGenerator<DynamicsTouchVectorGenerator.State, Vector3>
    {
        public class State
        {
            public TransformDynamicsMonitor Monitor { get; set; }
        }
        
        [SerializeField] private TransformDynamics.Order _order;

        [SerializeField] private TransformDynamics.Domain _domain;

        protected override bool BeginTrigger(ITouchData touchData, State state, out Vector3 value)
        {
            TransformDynamicsMonitor monitor = touchData.Actor.GetComponentInParent<TransformDynamicsMonitor>();
            if (monitor)
            {
                state.Monitor = monitor;
                value = monitor.Select(_order).Select(_domain);
                return true;
            }
            value = default;
            return false;;
        }

        protected override bool UpdateTrigger(ITouchData touchData, State state, Vector3 initial, Vector3 previous, out Vector3 value)
        {
            value = state.Monitor.Select(_order).Select(_domain);
            return true;
        }

        protected override Vector3 Relative(Vector3 initial, Vector3 current) => current - initial;
    }
}