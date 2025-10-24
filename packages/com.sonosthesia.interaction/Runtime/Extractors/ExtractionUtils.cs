using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum ExtractionSpace
    {
        World,
        Source,
        Actor
    }
    
    public enum VelocityExtractionType
    {
        Actor,
        Source,
        Relative
    }
    
    public static class ExtractionUtils
    {
        // consider space argument
        public static bool ExtractRelativePosition<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = e.Actor.Transform.position - e.Source.Transform.position;
            return true;
        }
        
        public static bool ExtractAxis<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            Vector3 actorToSource = e.Source.Transform.position - e.Actor.Transform.position;
            Vector3 actorVelocity = e.Actor.DynamicsMonitor.Velocity.Position;
            value = Vector3.Cross(actorVelocity, actorToSource);
            return true;
        }
        
        public static bool ExtractDirection<TEvent>(this TEvent e, ExtractionSpace space, Vector3 direction, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = space switch
            {
                ExtractionSpace.Source => e.Source.Transform.TransformDirection(direction),
                ExtractionSpace.Actor => e.Actor.Transform.TransformDirection(direction),
                _ => direction
            };
            return true;
        }
        
        public static bool ExtractVelocity<TEvent>(this TEvent e, VelocityExtractionType velocityType, out float value)
            where TEvent : IInteractionEvent
        {
            if (ExtractVelocity(e, velocityType, out Vector3 velocity))
            {
                value = velocity.magnitude;
                return true;
            }
            value = 0;
            return false;
        }
        
        public static bool ExtractVelocity<TEvent>(this TEvent e, VelocityExtractionType velocityType, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = default;
            switch (velocityType)
            {
                case VelocityExtractionType.Actor:
                    if (!e.Actor.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = e.Actor.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Source:
                    if (!e.Source.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Relative:
                    if (!e.Source.DynamicsMonitor || !e.Actor.DynamicsMonitor)
                    {
                        return false;
                    }
                    value = e.Actor.DynamicsMonitor.Velocity.Position -
                            e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
            }
            return false;
        }
    }
}