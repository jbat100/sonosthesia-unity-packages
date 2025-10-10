using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMPENoteConfiguration", menuName = "Sonosthesia/Instrument/TouchMPENoteConfiguration")]
    public class TouchMPENoteConfiguration : ScriptableObject
    {
        [SerializeField] private bool _applyPressure;
        public bool ApplyPressure => _applyPressure;
        
        [SerializeField] private bool _applySlide;
        public bool ApplySlide => _applySlide;
        
        [SerializeField] private bool _applyBend;
        public bool ApplyBend => _applyBend;
        
        [SerializeField] private MIDIPitchTouchExtractorSettings _pitch;
        public MIDIPitchTouchExtractorSettings Pitch => _pitch;

        [SerializeField] private FloatTouchDynamicExtractorSettings _velocity;
        public FloatTouchDynamicExtractorSettings Velocity => _velocity;

        [SerializeField] private FloatTouchDynamicExtractorSettings _pressure;
        public FloatTouchDynamicExtractorSettings Pressure => _pressure;
        
        [SerializeField] private FloatTouchDynamicExtractorSettings _slide;
        public FloatTouchDynamicExtractorSettings Slide => _slide;
        
        [SerializeField] private FloatTouchDynamicExtractorSettings _bend;
        public FloatTouchDynamicExtractorSettings Bend => _bend;
    }
}