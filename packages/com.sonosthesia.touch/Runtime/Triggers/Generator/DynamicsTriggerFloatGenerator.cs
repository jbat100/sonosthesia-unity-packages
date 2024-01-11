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

        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private FloatProcessor _postProcessor;

        protected override bool BeginTrigger(ITriggerData triggerData, State state, out float value)
        {
            TransformDynamicsMonitor monitor = triggerData.Actor.GetComponentInParent<TransformDynamicsMonitor>();
            if (monitor)
            {
                state.Monitor = monitor;
                value = Extract(monitor);
                return true;
            }
            value = default;
            return false;
        }

        protected override bool UpdateTrigger(ITriggerData triggerData, State state, float initial, float previous, out float value)
        {
            value = Extract(state.Monitor);
            return true;
        }

        protected virtual float Extract(TransformDynamicsMonitor monitor)
        {
            Vector3 v = monitor.Select(_order).Select(_domain);
            Vector3 local = transform.InverseTransformDirection(v);
            return _extractor.ExtractFloat(local);
        }
        
        protected override float Relative(float initial, float current) => current - initial;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
    }
}