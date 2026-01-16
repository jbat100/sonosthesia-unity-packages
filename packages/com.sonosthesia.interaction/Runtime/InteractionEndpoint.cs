using System.Collections.Generic;
using Sonosthesia.Dynamic;
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
        
        [SerializeField] private bool _mute;
        public bool Mute => _mute;
        
        [SerializeField] private List<InteractionGate> _gates;
        public IReadOnlyList<InteractionGate> Gates => _gates.AsReadOnly();
        
        Transform IInteractionEndpoint.Transform => transform; 
        
        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        protected virtual void Awake()
        {
            if (!_dynamicsMonitor)
            {
                _dynamicsMonitor = this.GetOrAddComponent<TransformDynamicsMonitor>();
            }
        }
    }
}