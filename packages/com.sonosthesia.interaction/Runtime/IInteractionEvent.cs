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
        public static Vector3 ActorPositionInSourceSpace(this IInteractionEvent touchEvent)
        {
            return touchEvent.Source.Transform
                .InverseTransformPoint(touchEvent.Actor.Transform.position);
        }
    }
}