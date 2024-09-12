using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DirectionTriggerGate : TriggerGate
    {
        [SerializeField] private Vector3 _direction;

        [SerializeField] private float _velocityThreshold;
        
        [SerializeField] private float _dotThreshold;

        [SerializeField] private bool _local;
        
        public override bool AllowTrigger(TriggerEndpoint source, TriggerEndpoint actor)
        {
            TransformDynamicsMonitor monitor = actor.GetComponentInParent<TransformDynamicsMonitor>();
            if (!monitor)
            {
                return false;
            }
            
            Vector3 velocity = monitor.Velocity.Position;
            if (velocity.magnitude < _velocityThreshold)
            {
                return false;
            }

            Vector3 localDirection = _local ? transform.TransformDirection(_direction) : _direction;
            if (Vector3.Dot(localDirection.normalized, velocity.normalized) < _dotThreshold)
            {
                return false;
            }
            
            return true;
        }
    }
}