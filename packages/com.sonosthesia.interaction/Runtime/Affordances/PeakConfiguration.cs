using System;
using Sonosthesia.Scheduler;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface IPeakConfiguration<in TEvent>
    {
        float Randomization { get; }
        SchedulerSettings Scheduler { get; }
        IInteractiveEnvelopeSettings<TEvent> Speed { get; }
        IInteractiveEnvelopeSettings<TEvent> Chaos { get; }
        IInteractiveEnvelopeSettings<TEvent> Magnitude { get; }
        IInteractiveEnvelopeSettings<TEvent> Duration { get; }
    }
    
    [Serializable]
    public class PeakConfiguration<TEvent, TEnvelope> : ScriptableObject, IPeakConfiguration<TEvent>
        where TEvent : struct
        where TEnvelope : IInteractiveEnvelopeSettings<TEvent> 
    {
        [SerializeField] [Range(0, 1)] private float _randomization;
        public float Randomization => _randomization;
        
        [SerializeField] private SchedulerSettings _scheduler;
        public SchedulerSettings Scheduler => _scheduler;

        [SerializeField] private TEnvelope _speed;
        public IInteractiveEnvelopeSettings<TEvent>  Speed => _speed;
        
        [SerializeField] private TEnvelope _chaos;
        public IInteractiveEnvelopeSettings<TEvent>  Chaos => _chaos;

        [SerializeField] private TEnvelope _magnitude;
        public IInteractiveEnvelopeSettings<TEvent>  Magnitude => _magnitude;

        [SerializeField] private TEnvelope _duration;
        public IInteractiveEnvelopeSettings<TEvent>  Duration => _duration;
    }
}