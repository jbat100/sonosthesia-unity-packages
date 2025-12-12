using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class DomainInteractionGate : EndpointInteractionGate
    {
        [SerializeField] private List<int> _domains;
        
        protected override bool PerformCheck(IInteractionEndpoint endpoint)
        {
            return _domains.Contains(endpoint.Domain);
        }
    }
}