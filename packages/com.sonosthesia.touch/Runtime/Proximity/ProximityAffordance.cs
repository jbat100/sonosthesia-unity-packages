using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ProximityAffordance : MonoBehaviour
    {
        private readonly Dictionary<Collider, ProximityZone> _zones = new ();
        protected IReadOnlyDictionary<Collider, ProximityZone> Zones => _zones;

        protected bool ClosestZone(Vector3 point, out ProximityZone zone, out Vector3 target, out float distance)
        {
            bool result = _zones.Count > 0;
            zone = null;
            target = default;
            distance = float.MaxValue;
            float closestSqr = float.MaxValue;
            bool found = false;
            foreach (ProximityZone candidateZone in _zones.Values)
            {
                if (!candidateZone.ComputeTarget(point, out Vector3 candidateTarget))
                {
                    continue;
                }
                float distanceSqr = (point - candidateTarget).sqrMagnitude;
                if (!(distanceSqr < closestSqr))
                {
                    continue;
                }
                found = true;
                closestSqr = distanceSqr;
                zone = candidateZone;
                target = candidateTarget;
            }

            if (found)
            {
                distance = Mathf.Sqrt(closestSqr);
            }
            return result;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            ProximityZone zone = other.GetComponent<ProximityZone>();
            if (!zone)
            {
                return;
            }

            _zones[other] = zone;
        }
        
        protected virtual void OnTriggerStay(Collider other)
        {
            
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            _zones.Remove(other);
        }
    }
}