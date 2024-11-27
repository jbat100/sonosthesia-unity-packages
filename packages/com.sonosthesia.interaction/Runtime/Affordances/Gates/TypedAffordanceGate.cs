using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class TypedAffordanceGate<TEvent> : InteractionAffordanceGate, 
        IInteractionAffordanceGate<TEvent> where TEvent : IInteractionEvent 
    {
        [SerializeField] private bool _bypass;

        public bool Check(TEvent e) => _bypass || PerformCheck(e);

        protected abstract bool PerformCheck(TEvent e);
    }
}