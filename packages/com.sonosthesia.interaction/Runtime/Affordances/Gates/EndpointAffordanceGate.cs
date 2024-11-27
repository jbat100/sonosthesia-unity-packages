using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class EndpointAffordanceGate : AffordanceGate
    {
        [SerializeField] private bool _source;
        [SerializeField] private bool _actor;
        
        protected sealed override bool PerformCheck(IInteractionEvent e)
        {
            if (_source && !PerformCheck(e.Source, e))
            {
                return false;
            }

            if (_actor && !PerformCheck(e.Actor, e))
            {
                return false;
            }

            return true;
        }

        protected abstract bool PerformCheck(IInteractionEndpoint endpoint, IInteractionEvent e);


    }
}