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