using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public interface IMPENoteConfiguration<in TEvent>
    {
        bool ApplyPressure { get; }
        
        bool ApplySlide { get; }
        
        bool ApplyBend { get; }
        
        IMIDIPitchExtractor<TEvent> Pitch { get; }
        
        IStaticExtractor<TEvent, float> Velocity { get; }
        
        IDynamicExtractor<TEvent, float> Pressure { get; }
        
        IDynamicExtractor<TEvent, float> Slide { get; }
        
        IDynamicExtractor<TEvent, float> Bend { get; }
    }
    
    public class MPENoteConfiguration<TEvent, TPitchExtractor, TStaticExtractor, TDynamicExtractor> 
        : ScriptableObject, IMPENoteConfiguration<TEvent>
        where TPitchExtractor : IMIDIPitchExtractor<TEvent>
        where TStaticExtractor : IStaticExtractor<TEvent, float> 
        where TDynamicExtractor : IDynamicExtractor<TEvent, float>
    {
        [SerializeField] private bool _applyPressure;
        public bool ApplyPressure => _applyPressure;
        
        [SerializeField] private bool _applySlide;
        public bool ApplySlide => _applySlide;
        
        [SerializeField] private bool _applyBend;
        public bool ApplyBend => _applyBend;
        
        [SerializeField] private TPitchExtractor _pitch;
        public IMIDIPitchExtractor<TEvent> Pitch => _pitch;

        [SerializeField] private TStaticExtractor _velocity;
        public IStaticExtractor<TEvent, float> Velocity => _velocity;

        [SerializeField] private TDynamicExtractor _pressure;
        public IDynamicExtractor<TEvent, float> Pressure => _pressure;
        
        [SerializeField] private TDynamicExtractor _slide;
        public IDynamicExtractor<TEvent, float> Slide => _slide;
        
        [SerializeField] private TDynamicExtractor _bend;
        public IDynamicExtractor<TEvent, float> Bend => _bend;
    }
}