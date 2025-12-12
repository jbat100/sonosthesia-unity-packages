using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class EndpointInteractionGate : InteractionGate
    {
        [SerializeField] private bool _source;
        [SerializeField] private bool _actor;
        
        protected sealed override bool PerformCheck(IInteractionEndpoint source, IInteractionEndpoint actor)
        {
            if (_source && !PerformCheck(source))
            {
                return false;
            }

            if (_actor && !PerformCheck(actor))
            {
                return false;
            }

            return true;
        }

        protected abstract bool PerformCheck(IInteractionEndpoint endpoint);
    }
}