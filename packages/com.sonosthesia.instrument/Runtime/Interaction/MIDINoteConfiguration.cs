using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public interface IMIDINoteConfiguration<in TEvent>
    {
        bool ApplyPressure { get; }
        
        IMIDIChannelExtractor<TEvent> Channel { get; }
        
        IMIDIPitchExtractor<TEvent> Pitch { get; }
        
        IStaticExtractor<TEvent, float> Velocity { get; }
        
        IDynamicExtractor<TEvent, float> Pressure { get; }
    }
    
    public class MIDINoteConfiguration<TEvent, TChannelExtractor, TPitchExtractor, TStaticExtractor, TDynamicExtractor> 
        : ScriptableObject, IMIDINoteConfiguration<TEvent>
        where TChannelExtractor : IMIDIChannelExtractor<TEvent> where TPitchExtractor : IMIDIPitchExtractor<TEvent>
        where TStaticExtractor : IStaticExtractor<TEvent, float> where TDynamicExtractor : IDynamicExtractor<TEvent, float>
    {
        [SerializeField] private bool _applyPressure;
        public bool ApplyPressure => _applyPressure;
        
        [SerializeField] private TChannelExtractor _channel;
        public IMIDIChannelExtractor<TEvent> Channel => _channel;

        [SerializeField] private TPitchExtractor _pitch;
        public IMIDIPitchExtractor<TEvent> Pitch => _pitch;

        [SerializeField] private TStaticExtractor _velocity;
        public IStaticExtractor<TEvent, float> Velocity => _velocity;

        [SerializeField] private TDynamicExtractor _pressure;
        public IDynamicExtractor<TEvent, float> Pressure => _pressure;
    }
}