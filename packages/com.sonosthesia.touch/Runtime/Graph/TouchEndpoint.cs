using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Dynamic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchEndpoint : MonoBehaviour
    {
        protected class TouchData : ITouchData
        {
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public TouchSource Source { get; set; }
            public TouchActor Actor { get; set; }
        }
        
        // can be used to filter actors or to allow one source to have different responses 
        [SerializeField] private int _domain;

        [SerializeField] private TouchNode _node;
        public TouchNode Node => _node;

        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        [SerializeField] private List<TouchGate> _gates;

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