using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum InteractionExtractorOrigin
    {
        Self,
        Source,
        Actor
    }
    
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
        public static TComponent GetComponent<TEvent, TComponent>(this TEvent e, InteractionExtractorOrigin origin, TComponent self)
            where TEvent : IInteractionEvent 
        {
            return origin switch
            {
                InteractionExtractorOrigin.Self => self,
                InteractionExtractorOrigin.Source => e.Source.Transform.GetComponent<TComponent>(),
                InteractionExtractorOrigin.Actor => e.Actor.Transform.GetComponent<TComponent>(),
                _ => throw new NotSupportedException()
            };
        }
        
        public static bool WorldToExtractionSpace<TEvent>(this TEvent e, ExtractionSpace extractionSpace, Vector3 point, out Vector3 result)
            where TEvent : IInteractionEvent
        {
            result = default;
            switch (extractionSpace)
            {
                case ExtractionSpace.World:
                    result = point;
                    return true;
                case ExtractionSpace.Actor when e.Actor?.Transform !=null:
                    result = e.Actor.Transform.InverseTransformPoint(point);
                    return true;
                case ExtractionSpace.Source when e.Source?.Transform != null:
                    result = e.Source.Transform.InverseTransformPoint(point);
                    return true;
                default:
                    return false;
            }
        }
        
        // consider space argument
        public static bool ExtractRelativePosition<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            if (!e.HasValidTransforms())
            {
                value = default;
                return false;
            }
            value = e.Actor.Transform.position - e.Source.Transform.position;
            return true;
        }

        public static bool ExtractDistance<TEvent>(this TEvent e, Axes axes, out float value)
            where TEvent : IInteractionEvent
        {
            return e.ActorToSourceDistance(axes, out value);
        }
        
        public static bool ExtractAxis<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            if (!e.HasValidTransforms() || !e.Actor.DynamicsMonitor)
            {
                value = default;
                return false;
            }
            Vector3 actorToSource = e.Source.Transform.position - e.Actor.Transform.position;
            Vector3 actorVelocity = e.Actor.DynamicsMonitor.Velocity.Position;
            value = Vector3.Cross(actorVelocity, actorToSource);
            return true;
        }
        
        public static bool TransformPoint<TEvent>(this TEvent e, ExtractionSpace space, Vector3 direction, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = default;
            switch (space)
            {
                case ExtractionSpace.Actor when e.Actor?.Transform != null:
                    value = e.Actor.Transform.TransformPoint(direction);
                    return true;
                case ExtractionSpace.Source when e.Source?.Transform != null:
                    value = e.Source.Transform.TransformPoint(direction);
                    return true;
                default:
                    return false;
            }
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
                case VelocityExtractionType.Actor when e.Actor?.DynamicsMonitor != null:
                    value = e.Actor.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Source when e.Source?.DynamicsMonitor != null:
                    value = e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Relative when e.Actor?.DynamicsMonitor != null && e.Source?.DynamicsMonitor != null:
                    value = e.Actor.DynamicsMonitor.Velocity.Position - e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                default:
                    return false;
            }
        }
    }
}