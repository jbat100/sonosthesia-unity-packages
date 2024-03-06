using UnityEngine;

namespace Sonosthesia.Touch
{
    public class PointProximityZone : ProximityZone
    {
        [SerializeField] private Vector3 _offset;
        
        public override bool ComputeTarget(Vector3 point, out Vector3 target)
        {
            target = transform.TransformPoint(_offset);
            return true;
        }
    }
}