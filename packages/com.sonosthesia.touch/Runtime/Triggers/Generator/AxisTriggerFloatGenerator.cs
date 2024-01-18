using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AxisTriggerFloatGenerator : StatefulTriggerValueGenerator<AxisTriggerFloatGenerator.State, float>
    {
        public class State
        {
            internal Vector3 Initial { get; set; }
        }

        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private FloatProcessor _postProcessor;
        
        protected override bool BeginTrigger(ITriggerData triggerData, State state, out float value)
        {
            Vector3 position = ComputeOtherLocalPosition(triggerData);
            state.Initial = position;
            value = _extractor.ExtractFloat(position);
            return true;
        }

        protected override bool UpdateTrigger(ITriggerData triggerData, State state, float initial, float previous, out float value)
        {
            Vector3 position = ComputeOtherLocalPosition(triggerData);
            value = _extractor.ExtractFloat(position);
            return true;
        }

        protected virtual Vector3 ComputeOtherLocalPosition(ITriggerData triggerData)
        {
            Collider other = triggerData.Collider;
            Vector3 position = other.attachedRigidbody ? other.attachedRigidbody.position : other.transform.position;
            return transform.InverseTransformPoint(position);
        }

        protected override float Relative(float initial, float current) => current - initial;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
    }
}