using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum TriggerInteraction
    {
        Pulse,
        Hold
    }

    public interface IInteractiveTriggerSettings<in TEvent>
    {
        TriggerInteraction Interaction { get; }
        IDynamicExtractor<TEvent, float> ValueExtractor { get; }
        IStaticExtractor<TEvent, float> AttackExtractor { get; }
        IStaticExtractor<TEvent, float> ReleaseExtractor { get; }
        EnvelopeSettings Envelope { get; }
        OneEuroFilterSettings OneEuroFilter { get; }
        EaseType ReleaseType { get; }
    }
    
    [Serializable]
    public class InteractiveTriggerSettings<TEvent, TDynamicExtractor, TStaticExtractor> : IInteractiveTriggerSettings<TEvent> 
        where TEvent : struct
        where TDynamicExtractor : IDynamicExtractor<TEvent, float>
        where TStaticExtractor : IStaticExtractor<TEvent, float>
    {
        [SerializeField] private OneEuroFilterSettings _oneEuroFilter;
        public OneEuroFilterSettings OneEuroFilter => _oneEuroFilter;
        
        [SerializeField] private TriggerInteraction _interaction;
        public TriggerInteraction Interaction => _interaction;
        
        [SerializeField] private TDynamicExtractor _valueExtractor;
        public IDynamicExtractor<TEvent, float> ValueExtractor => _valueExtractor;
        
        [SerializeField] private TStaticExtractor _attackExtractor;
        public IStaticExtractor<TEvent, float> AttackExtractor => _attackExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope
            = EnvelopeSettings.ADS(EnvelopePhase.InOutSine(0.1f), EnvelopePhase.InOutSine(0.3f), 0.5f);
        public EnvelopeSettings Envelope => _envelope;

        [SerializeField] private TStaticExtractor _releaseExtractor;
        public IStaticExtractor<TEvent, float> ReleaseExtractor => _releaseExtractor;

        [SerializeField] private EaseType _releaseType = EaseType.easeInOutSine;
        public EaseType ReleaseType => _releaseType;
    }
}