using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public interface ITriggerConfiguration<in TEvent>
    {
        IInteractiveTriggerSettings<TEvent> Settings { get; }
    }
    
    public class TriggerConfiguration<TEvent, TDynamicExtractor, TStaticExtractor> : ScriptableObject, ITriggerConfiguration<TEvent>
        where TEvent : struct 
        where TDynamicExtractor : IDynamicExtractor<TEvent, float> 
        where TStaticExtractor : IStaticExtractor<TEvent, float>
    {
        [SerializeField] private InteractiveTriggerSettings<TEvent, TDynamicExtractor, TStaticExtractor> _settings;
        public IInteractiveTriggerSettings<TEvent> Settings => _settings;
    }
}