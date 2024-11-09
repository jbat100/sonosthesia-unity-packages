using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface IAffordanceGate
    {
        public bool Check(IInteractionEvent e);
    }
    
    public interface IAffordanceGate<in TEvent> where TEvent : IInteractionEvent
    {
        public bool Check(TEvent e);
    }
    
    public abstract class AbstractAffordanceGate : MonoBehaviour
    {
        
    }
}