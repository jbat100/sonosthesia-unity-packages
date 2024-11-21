using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface IInteractionAffordanceGate
    {
        public bool Check(IInteractionEvent e);
    }
    
    public interface IInteractionAffordanceGate<in TEvent> where TEvent : IInteractionEvent
    {
        public bool Check(TEvent e);
    }
    
    public abstract class InteractionAffordanceGate : MonoBehaviour
    {
        
    }
}