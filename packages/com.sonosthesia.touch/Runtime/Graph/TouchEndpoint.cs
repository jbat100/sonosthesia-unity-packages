using System;
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
        
        [SerializeField] private int _domain;
        public int Domain => _domain;        
        
        [SerializeField] private InteractionLayerMask _interactionLayers = ~0;
        public InteractionLayerMask InteractionLayers => _interactionLayers;

        [SerializeField] private TouchNode _node;
        public TouchNode Node => _node;

        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        [SerializeField] private List<TouchActorGate> _gates;
        public IReadOnlyList<TouchActorGate> Gates => _gates.AsReadOnly();

        private float? _enableTime;
        public float? TimeSinceEnable => Time.time - _enableTime;
        
        protected virtual void Awake()
        {
            if (!_dynamicsMonitor)
            {
                _dynamicsMonitor = this.GetOrAddComponent<TransformDynamicsMonitor>();
            }
        }

        protected virtual void OnEnable()
        {
            _enableTime = Time.time;
        }

        protected virtual void OnDisable()
        {
            _enableTime = null;
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