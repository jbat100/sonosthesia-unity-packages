using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchTriggerAffordanceConfiguration", menuName = "Sonosthesia/Touch/TouchTriggerAffordanceConfiguration")]
    public class TouchTriggerAffordanceConfiguration : ScriptableObject
    {
        [SerializeField] private FloatTouchExtractorSettings _valueScaleExtractor;
        public FloatTouchExtractorSettings ValueScaleExtractor => _valueScaleExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _timeScaleExtractor;
        public FloatTouchExtractorSettings TimeScaleExtractor => _timeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _envelope;
        public EnvelopeSettings Envelope => _envelope;
    }
}