using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AxisThresholdTouchFloatGenerator : ThresholdTouchFloatGenerator
    {
        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private FloatProcessor _postProcessor;
        protected override float Relative(float initial, float current) => current - initial;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
        
        protected override bool Extract(ITouchData touchData, out float value)
        {
            Collider other = touchData.Collider;
            Vector3 position = other.attachedRigidbody ? other.attachedRigidbody.position : other.transform.position;
            value = _extractor.ExtractFloat(transform.InverseTransformPoint(position));
            return true;
        }
    }
}