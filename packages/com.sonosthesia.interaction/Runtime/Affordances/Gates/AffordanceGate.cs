using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class AffordanceGate : AbstractAffordanceGate, IAffordanceGate
    {
        [SerializeField] private bool _bypass;

        public bool Check(IInteractionEvent e) => _bypass || PerformCheck(e);

        protected abstract bool PerformCheck(IInteractionEvent e);
    }
}