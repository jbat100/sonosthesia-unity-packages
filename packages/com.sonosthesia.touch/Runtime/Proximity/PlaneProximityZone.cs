using UnityEngine;

namespace Sonosthesia.Touch
{
    public class PlaneProximityZone : ProximityZone
    {
        [SerializeField] private Vector3 _normal = Vector3.up;

        [SerializeField] private Vector3 _offset;

        protected override bool ComputeRawTarget(Vector3 point, out Vector3 target)
        {
            Plane plane = new Plane(transform.TransformDirection(_normal), transform.TransformPoint(_offset));
            target = plane.ClosestPointOnPlane(point);
            return true;
        }
    }
}