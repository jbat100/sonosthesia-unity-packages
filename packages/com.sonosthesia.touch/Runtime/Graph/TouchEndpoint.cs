using System.Collections.Generic;
using Sonosthesia.Dynamic;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Touch
{
    public abstract class TouchEndpoint : MonoBehaviour, IInteractionEndpoint, ILogSwitch
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        [SerializeField] private bool _mute;
        public bool Mute => _mute;
        
        [SerializeField] private InteractionLayerMask _interactionLayers = ~0;
        public InteractionLayerMask InteractionLayers => _interactionLayers;

        [SerializeField] private TouchNode _node;
        public TouchNode Node => _node;

        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        [SerializeField] private List<TouchGate> _gates;
        public IReadOnlyList<TouchGate> Gates => _gates.AsReadOnly();

        protected virtual void Awake()
        {
            if (!_dynamicsMonitor)
            {
                _dynamicsMonitor = this.GetOrAddComponent<TransformDynamicsMonitor>();
            }
        }

        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            return !_node || _node.RequestPermission(other);
        }
    }
}