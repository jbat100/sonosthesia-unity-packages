using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class TypedAffordanceGate<TEvent> : AbstractAffordanceGate, 
        IAffordanceGate<TEvent> where TEvent : IInteractionEvent 
    {
        [SerializeField] private bool _bypass;

        public bool Check(TEvent e) => _bypass || PerformCheck(e);

        protected abstract bool PerformCheck(TEvent e);
    }
}