using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum EnvelopeInteraction
    {
        Constant,
        Pulse,
        Contact
    }
    
    public enum EnvelopeFilter
    {
        None,
        OneEuro
    }
    
    public interface IInteractiveEnvelopeSettings<in TEvent>
    {
        EnvelopeInteraction Interaction { get; }
        IDynamicExtractor<TEvent, float> ValueScaleExtractor { get; }
        IStaticExtractor<TEvent, float> TimeScaleExtractor { get; }
        IStaticExtractor<TEvent, float> ReleaseExtractor { get; }
        EnvelopeSettings Envelope { get; }
        EnvelopeFilter Filter { get; }
        OneEuroFilterSettings OneEuroFilter { get; }
        EaseType ReleaseType { get; }
    }
    
    [Serializable]
    public class InteractiveEnvelopeSettings<TEvent, TDynamicExtractor, TStaticExtractor> : IInteractiveEnvelopeSettings<TEvent> 
        where TEvent : struct
        where TDynamicExtractor : IDynamicExtractor<TEvent, float>
        where TStaticExtractor : IStaticExtractor<TEvent, float>
    {
        [SerializeField] private EnvelopeFilter _filter;
        public EnvelopeFilter Filter => _filter;

        [SerializeField] private OneEuroFilterSettings _oneEuroFilter;
        public OneEuroFilterSettings OneEuroFilter => _oneEuroFilter;
        
        [SerializeField] private EnvelopeInteraction _interaction;
        public EnvelopeInteraction Interaction => _interaction;
        
        [SerializeField] private TDynamicExtractor _valueScaleExtractor;
        public IDynamicExtractor<TEvent, float> ValueScaleExtractor => _valueScaleExtractor;
        
        [SerializeField] private TStaticExtractor _timeScaleExtractor;
        public IStaticExtractor<TEvent, float> TimeScaleExtractor => _timeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope
            = EnvelopeSettings.ADS(EnvelopePhase.InOutSine(0.1f), EnvelopePhase.InOutSine(0.3f), 0.5f);
        public EnvelopeSettings Envelope => _envelope;

        [SerializeField] private TStaticExtractor _releaseExtractor;
        public IStaticExtractor<TEvent, float> ReleaseExtractor => _releaseExtractor;

        [SerializeField] private EaseType _releaseType = EaseType.easeInOutSine;
        public EaseType ReleaseType => _releaseType;
    }
}