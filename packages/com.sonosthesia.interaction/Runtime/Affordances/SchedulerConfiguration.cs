using System;
using Sonosthesia.Scheduler;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface ISchedulerConfiguration<in TEvent>
    {
        SchedulerSettings Scheduler { get; }
        IInteractiveEnvelopeSettings<TEvent> Speed { get; }
        IInteractiveEnvelopeSettings<TEvent> Chaos { get; }
    }
    
    [Serializable]
    public class SchedulerConfiguration<TEvent, TEnvelope> : ScriptableObject, ISchedulerConfiguration<TEvent>
        where TEvent : struct
        where TEnvelope : IInteractiveEnvelopeSettings<TEvent> 
    {
        [SerializeField] private SchedulerSettings _scheduler;
        public SchedulerSettings Scheduler => _scheduler;

        [SerializeField] private TEnvelope _speed;
        public IInteractiveEnvelopeSettings<TEvent>  Speed => _speed;
        
        [SerializeField] private TEnvelope _chaos;
        public IInteractiveEnvelopeSettings<TEvent>  Chaos => _chaos;
    }
}