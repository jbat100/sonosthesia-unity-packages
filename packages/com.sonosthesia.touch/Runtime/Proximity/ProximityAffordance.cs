using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ProximityAffordance : MonoBehaviour
    {
        private readonly Dictionary<Collider, ProximityZone> _zones = new ();
        protected IReadOnlyDictionary<Collider, ProximityZone> Zones => _zones;

        protected abstract float DistanceToZone(ProximityZone zone);

        protected bool ClosestZone(out ProximityZone zone, out float distance)
        {
            bool result = _zones.Count > 0;
            zone = null;
            distance = float.MaxValue;
            float closest = float.MaxValue;
            foreach (ProximityZone candidate in _zones.Values)
            { 
                float d = DistanceToZone(candidate);
                if (d < closest)
                {
                    closest = d;
                    zone = candidate;
                }
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