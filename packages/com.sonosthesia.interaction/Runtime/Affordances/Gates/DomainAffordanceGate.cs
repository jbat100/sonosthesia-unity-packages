using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class DomainAffordanceGate : EndpointAffordanceGate
    {
        [SerializeField] private List<int> _domains;
        
        protected override bool PerformCheck(IInteractionEndpoint endpoint, IInteractionEvent e)
        {
            return _domains.Contains(endpoint.Domain);
        }
    }
}