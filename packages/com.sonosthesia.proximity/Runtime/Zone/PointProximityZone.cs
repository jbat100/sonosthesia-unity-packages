using UnityEngine;

namespace Sonosthesia.Proximity
{
    public class PointProximityZone : ProximityZone
    {
        [SerializeField] private Vector3 _offset;
        
        protected override bool ComputeRawTarget(Vector3 point, out Vector3 target)
        {
            target = transform.TransformPoint(_offset);
            return true;
        }
    }
}