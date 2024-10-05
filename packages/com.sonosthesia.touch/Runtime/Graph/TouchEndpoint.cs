using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Dynamic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// ITriggerData plays the same role as PointerEventData
    /// </summary>

    public interface ITouchData
    {
        Collider Collider { get; }
        bool Colliding { get; }
        TouchSource Source { get; }
        TouchActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TouchEvent
    {
        public readonly Guid Id;
        public readonly ITouchData TouchData;
        public readonly float StartTime;

        public TouchEvent(Guid id, ITouchData touchData, float startTime)
        {
            Id = id;
            TouchData = touchData;
            StartTime = startTime;
        }

        public void EndStream()
        {
            TouchData.Source.RequestKillStream(Id);
        }
    }
    
    public abstract class TouchEndpoint : TouchStream
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
        
        // Only source endpoints can kill streams so endpoints redirect kill requests
        
        public void RequestKillAllStreams()
        {
            Dictionary<Guid, TouchEvent> events = new Dictionary<Guid, TouchEvent>(EventStreamNode.Values);
            foreach (KeyValuePair<Guid, TouchEvent> pair in events)
            {
                if (pair.Value.TouchData.Source is ITriggerSource source)
                {
                    source.KillStream(pair.Key);   
                }
            }
        }

        public void RequestKillStream(Guid id)
        {
            if (EventStreamNode.Values.TryGetValue(id, out TouchEvent e))
            {
                if (e.TouchData.Source is ITriggerSource source)
                {
                    source.KillStream(id);
                }
            }
        }
        
        protected virtual void Awake()
        {
            if (_node)
            {
                _node.EventStreamNode.Pipe(EventStreamNode);    
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