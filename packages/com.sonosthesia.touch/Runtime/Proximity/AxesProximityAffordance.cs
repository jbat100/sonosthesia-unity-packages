using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AxesProximityAffordance : ProximityAffordance
    {
        [SerializeField] private Axes _distanceAxes = Axes.Y;
        
        protected override float DistanceToZone(ProximityZone zone)
        {
            Vector3 zoneSpacePosition = zone.transform.InverseTransformPoint(transform.position);
            Vector3 distance = zoneSpacePosition.FilterAxes(_distanceAxes);
            return distance.magnitude;
        }
    }
}