using UnityEngine;

namespace Sonosthesia.Proximity
{
    public class LineProximityZone : ProximityZone
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;

        [SerializeField] private Vector3 _offset;
        
        protected override bool ComputeRawTarget(Vector3 point, out Vector3 target)
        {
            // https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/
            
            Vector3 linePoint = transform.TransformPoint(_offset);
            Vector3 lineDirection = transform.TransformDirection(_direction).normalized;
            
            Vector3 v = point - linePoint;
            float d = Vector3.Dot(v, lineDirection);
            target = linePoint + lineDirection * d;

            return true;
        }
    }
}