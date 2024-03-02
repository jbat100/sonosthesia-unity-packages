using System;
using System.Collections.Generic;
using System.Linq;
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
        TriggerEndpoint Source { get; }
        TriggerEndpoint Actor { get; }
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
            TriggerData.Source.EndStream(Id);
        }
    }
    
    public abstract class TriggerEndpoint : TriggerStream
    {
        // can be used to filter actors or to allow one source to have different responses 
        [SerializeField] private int _domain;

        [SerializeField] private TriggerNode _node;
        public TriggerNode Node => _node;

        [SerializeField] private List<TriggerGate> _gates;

        public bool CheckGates(TriggerEndpoint source, TriggerEndpoint actor)
        {
            return _gates.All(gate => gate.AllowTrigger(this, actor));
        }
        
        public abstract void EndAllStreams();

        public abstract void EndStream(Guid id);
        
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