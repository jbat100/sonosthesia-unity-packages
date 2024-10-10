using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Dynamic;
using Sonosthesia.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Touch
{
    public abstract class TouchEndpoint : MonoBehaviour, IInteractionEndpoint
    {
        [SerializeField] private InteractionLayerMask _interactionLayers = ~0;
        public InteractionLayerMask InteractionLayers => _interactionLayers;

        [SerializeField] private TouchNode _node;
        public TouchNode Node => _node;

        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        [SerializeField] private List<TouchGate> _gates;

        protected virtual void Awake()
        {
            if (!_dynamicsMonitor)
            {
                // TODO: Auto add ?
                _dynamicsMonitor = GetComponent<TransformDynamicsMonitor>();
            }
        }
        
        public bool CheckGates(TouchEndpoint source, TouchEndpoint actor)
        {
            return _gates.All(gate => gate.AllowTrigger(this, actor));
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