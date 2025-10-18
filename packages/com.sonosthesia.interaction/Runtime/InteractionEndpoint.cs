using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    public class InteractionEndpoint : MonoBehaviour, IInteractionEndpoint, ILogSwitch
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        [SerializeField] private int _domain;
        public int Domain => _domain;        
        
        [SerializeField] private InteractionLayerMask _interactionLayers = ~0;
        public InteractionLayerMask InteractionLayers => _interactionLayers;
    }
}