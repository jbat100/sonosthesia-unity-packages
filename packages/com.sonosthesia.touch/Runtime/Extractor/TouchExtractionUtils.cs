using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public enum TouchVelocityType
    {
        Actor,
        Source,
        Relative
    }
    
    internal static class TouchExtractionUtils
    {
        internal static bool ExtractVelocity(TouchEvent touchEvent, TouchVelocityType velocityType, out float value)
        {
            if (ExtractVelocity(touchEvent, velocityType, out Vector3 velocity))
            {
                value = velocity.magnitude;
                return true;
            }
            value = 0;
            return false;
        }
        
        internal static bool ExtractVelocity(TouchEvent touchEvent, TouchVelocityType velocityType, out Vector3 value)
        {
            value = default;
            switch (velocityType)
            {
                case TouchVelocityType.Actor:
                    if (!touchEvent.touchData.Actor.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position;
                    return true;
                case TouchVelocityType.Source:
                    if (!touchEvent.touchData.Source.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                case TouchVelocityType.Relative:
                    if (!touchEvent.touchData.Source.DynamicsMonitor || !touchEvent.touchData.Actor.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position -
                            touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position;
                    return true;
            }
            return false;
        }
    }
}