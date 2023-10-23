using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AxisDragTriggerFloatGenerator : StatefulTriggerValueGenerator<AxisDragTriggerFloatGenerator.State, float>
    {
        public class State
        {
            internal Vector3 Initial { get; set; }
        }

        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private bool _relative;
        
        protected override bool ProcessTriggerEnter(Collider other, State state, out float value)
        {
            Vector3 position = ComputeOtherLocalPosition(other);
            state.Initial = position;
            value = _extractor.Process(_relative ? Vector3.zero : position);
            return true;
        }

        protected override bool ProcessTriggerStay(Collider other, State state, float initial, float previous, out float value)
        {
            Vector3 position = ComputeOtherLocalPosition(other);
            value = _extractor.Process(_relative ? position - state.Initial : position);
            return true;
        }

        protected virtual Vector3 ComputeOtherLocalPosition(Collider other)
        {
            Vector3 position = other.attachedRigidbody ? other.attachedRigidbody.position : other.transform.position;
            return transform.InverseTransformPoint(position);
        }
    }
}