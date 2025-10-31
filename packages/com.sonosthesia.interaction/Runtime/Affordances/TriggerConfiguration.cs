using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface ITriggerConfiguration<in TEvent>
    {
        IInteractiveEnvelopeSettings<TEvent> Settings { get; }
    }
    
    public class TriggerConfiguration<TEvent, TDynamicExtractor, TStaticExtractor> : ScriptableObject, ITriggerConfiguration<TEvent>
        where TEvent : struct 
        where TDynamicExtractor : IDynamicExtractor<TEvent, float> 
        where TStaticExtractor : IStaticExtractor<TEvent, float>
    {
        [SerializeField] private InteractiveEnvelopeSettings<TEvent, TDynamicExtractor, TStaticExtractor> _settings;
        public IInteractiveEnvelopeSettings<TEvent> Settings => _settings;
    }
}