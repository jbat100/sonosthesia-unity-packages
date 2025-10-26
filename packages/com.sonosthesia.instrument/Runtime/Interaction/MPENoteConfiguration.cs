using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Instrument
{
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