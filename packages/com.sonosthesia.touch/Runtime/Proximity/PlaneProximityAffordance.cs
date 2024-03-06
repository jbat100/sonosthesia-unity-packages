using UnityEngine;

namespace Sonosthesia.Touch
{
    public class PlaneProximityAffordance : ProximityAffordance
    {
        [SerializeField] private Vector3 _normal = Vector3.up;

        [SerializeField] private Vector3 _offset;
        
        protected Plane Plane => new Plane(transform.TransformDirection(_normal), transform.TransformPoint(_offset));

        protected override float DistanceToZone(ProximityZone zone)
        {
            return 0;
        }
    }
}