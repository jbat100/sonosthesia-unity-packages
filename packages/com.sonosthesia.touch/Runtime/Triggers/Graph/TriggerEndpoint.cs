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

    public interface ITriggerData
    {
        Collider Collider { get; }
        bool Colliding { get; }
        TriggerSource Source { get; }
        TriggerActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TriggerEvent
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;
        public readonly float StartTime;

        public TriggerEvent(Guid id, ITriggerData triggerData, float startTime)
        {
            Id = id;
            TriggerData = triggerData;
            StartTime = startTime;
        }

        public void EndStream()
        {
            TriggerData.Source.RequestKillStream(Id);
        }
    }
    
    public abstract class TriggerEndpoint : TriggerStream
    {
        protected class TriggerData : ITriggerData
        {
            public Collider Collider { get; set; }
            public bool Colliding { get; set; }
            public TriggerSource Source { get; set; }
            public TriggerActor Actor { get; set; }
        }
        
        // can be used to filter actors or to allow one source to have different responses 
        [SerializeField] private int _domain;

        [SerializeField] private TriggerNode _node;
        public TriggerNode Node => _node;

        [SerializeField] private TransformDynamicsMonitor _dynamicsMonitor;
        public TransformDynamicsMonitor DynamicsMonitor => _dynamicsMonitor;
        
        [SerializeField] private List<TriggerGate> _gates;

        public bool CheckGates(TriggerEndpoint source, TriggerEndpoint actor)
        {
            return _gates.All(gate => gate.AllowTrigger(this, actor));
        }
        
        // Only source endpoints can kill streams so endpoints redirect kill requests
        
        public void RequestKillAllStreams()
        {
            Dictionary<Guid, TriggerEvent> events = new Dictionary<Guid, TriggerEvent>(EventStreamNode.Values);
            foreach (KeyValuePair<Guid, TriggerEvent> pair in events)
            {
                if (pair.Value.TriggerData.Source is ITriggerSource source)
                {
                    source.KillStream(pair.Key);   
                }
            }
        }

        public void RequestKillStream(Guid id)
        {
            if (EventStreamNode.Values.TryGetValue(id, out TriggerEvent e))
            {
                if (e.TriggerData.Source is ITriggerSource source)
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