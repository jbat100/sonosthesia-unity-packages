using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    public class LayerAffordanceGate : EndpointAffordanceGate
    {
        [SerializeField] private InteractionLayerMask _layers;
        [SerializeField] private InteractionLayerMatch _match = InteractionLayerMatch.Pass;

        protected override bool PerformCheck(IInteractionEndpoint endpoint, IInteractionEvent e)
        {
            return _match.Match(_layers, endpoint.InteractionLayers);
        }
    }
}