using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AxisTriggerFloatGenerator : RelativeTriggerValueGenerator<float>
    {
        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private FloatProcessor _postProcessor;
        
        protected override bool Extract(ITriggerData triggerData, out float value)
        {
            Collider other = triggerData.Collider;
            Vector3 position = other.attachedRigidbody ? other.attachedRigidbody.position : other.transform.position;
            value = _extractor.ExtractFloat(transform.InverseTransformPoint(position));
            return true;
        }

        protected override float Relative(float initial, float current) => current - initial;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
    }
}