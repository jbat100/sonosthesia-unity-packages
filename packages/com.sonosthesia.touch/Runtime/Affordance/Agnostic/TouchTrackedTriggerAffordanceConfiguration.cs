using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchTrackedTriggerAffordanceConfiguration", 
        menuName = "Sonosthesia/Touch/TouchTrackedTriggerAffordanceConfiguration")]
    public class TouchTrackedTriggerAffordanceConfiguration : ScriptableObject
    {
        [SerializeField] private FloatTouchExtractorSettings _startValueScaleExtractor;
        public FloatTouchExtractorSettings StartValueScaleExtractor => _startValueScaleExtractor;
        
        [SerializeField] private FloatTouchExtractorSettings _startTimeScaleExtractor;
        public FloatTouchExtractorSettings StartTimeScaleExtractor => _startTimeScaleExtractor;
        
        [SerializeField] private EnvelopeSettings _startEnvelope
            = EnvelopeSettings.ADS(EnvelopePhase.Linear(0.5f), EnvelopePhase.Linear(0.5f), 0.5f);
        public EnvelopeSettings StartEnvelope => _startEnvelope;
        
        [SerializeField] private FloatTouchExtractorSettings _endTimeScaleExtractor;
        public FloatTouchExtractorSettings EndTimeScaleExtractor => _endTimeScaleExtractor;

        [SerializeField] private EnvelopeSettings _endEnvelope
            = EnvelopeSettings.SR(EnvelopePhase.Linear(1f)); 
        public EnvelopeSettings EndEnvelope => _endEnvelope;
    }
}