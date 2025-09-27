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

        [SerializeField] private FloatTouchExtractorSettings _velocity;
        public FloatTouchExtractorSettings Velocity => _velocity;

        [SerializeField] private FloatTouchExtractorSettings _pressure;
        public FloatTouchExtractorSettings Pressure => _pressure;
        
        [SerializeField] private FloatTouchExtractorSettings _slide;
        public FloatTouchExtractorSettings Slide => _slide;
        
        [SerializeField] private FloatTouchExtractorSettings _bend;
        public FloatTouchExtractorSettings Bend => _bend;
    }
}