using UnityEngine;

namespace Sonosthesia.Collide
{
    public enum CollisionVelocityType
    {
        Actor,
        Source,
        Relative
    }
    
    internal static class CollisionExtractionUtils
    {
        internal static bool ExtractVelocity(CollideEvent touchEvent, CollisionVelocityType velocityType, out float value)
        {
            if (ExtractVelocity(touchEvent, velocityType, out Vector3 velocity))
            {
                value = velocity.magnitude;
                return true;
            }
            value = 0;
            return false;
        }
        
        internal static bool ExtractVelocity(CollideEvent touchEvent, CollisionVelocityType velocityType, out Vector3 value)
        {
            value = default;
            switch (velocityType)
            {
                case CollisionVelocityType.Actor:
                    
                    return true;
                case CollisionVelocityType.Source:
                    return true;
                case CollisionVelocityType.Relative:
                    return true;
            }
            return false;
        }
    }
}