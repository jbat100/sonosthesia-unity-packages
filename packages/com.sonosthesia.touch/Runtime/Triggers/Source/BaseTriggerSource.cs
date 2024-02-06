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
        BaseTriggerSource Source { get; }
        BaseTriggerActor Actor { get; }
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
    
    public abstract class BaseTriggerSource : TriggerStream
    {
        [SerializeField] private List<TriggerGate> _gates;

        protected bool CheckGates(BaseTriggerActor actor)
        {
            return _gates.All(gate => gate.AllowTrigger(this, actor));
        }
        
        public abstract void EndAllStreams();

        public abstract void EndStream(Guid id);
    }
}