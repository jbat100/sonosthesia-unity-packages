using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    public class LayerAffordanceGate : AffordanceGate, IAffordanceGate
    {
        [SerializeField] private bool _source;
        [SerializeField] private bool _actor;
        
        [SerializeField] private InteractionLayerMask _layers;
        [SerializeField] private InteractionLayerMatch _match = InteractionLayerMatch.Pass;

        protected override bool PerformCheck(IInteractionEvent e)
        {
            if (_source && !_match.Match(_layers, e.Source.InteractionLayers))
            {
                return false;
            }

            if (_actor && !_match.Match(_layers, e.Actor.InteractionLayers))
            {
                return false;
            }

            return true;
        }
    }
}