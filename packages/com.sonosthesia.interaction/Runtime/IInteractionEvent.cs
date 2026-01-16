using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface IInteractionEvent
    {
        float StartTime { get; }
        
        IInteractionEndpoint Source { get; }
        IInteractionEndpoint Actor { get; }
    }
    
    public static class InteractionEventExtensions
    {
        public static bool HasValidTransforms(this IInteractionEvent e)
        {
            return e.Source?.Transform != null && e.Actor?.Transform != null;
        }
        
        public static bool ActorPositionInSourceSpace(this IInteractionEvent e, out Vector3 position)
        {
            if (!e.HasValidTransforms())
            {
                position = default;
                return false;
            }
            position = e.Source.Transform.InverseTransformPoint(e.Actor.Transform.position);
            return true;
        }

        public static bool ActorToSourceDistance(this IInteractionEvent e, Axes axes, out float distance)
        {
            if (!e.ActorPositionInSourceSpace(out Vector3 position))
            {
                distance = 0;
                return false;
            }
            distance = position.FilterAxes(axes).magnitude;
            return true;
        }
    }
}