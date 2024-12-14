using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMIDINoteConfiguration", menuName = "Sonosthesia/Touch/TouchMIDINoteConfiguration")]
    public class TouchMIDINoteConfiguration : ScriptableObject
    {
        [SerializeField] private bool _applyPressure;
        public bool ApplyPressure => _applyPressure;
        
        [SerializeField] private MIDIChannelTouchExtractorSettings _channel;
        public MIDIChannelTouchExtractorSettings Channel => _channel;

        [SerializeField] private MIDIPitchTouchExtractorSettings _pitch;
        public MIDIPitchTouchExtractorSettings Pitch => _pitch;

        [SerializeField] private FloatTouchExtractorSettings _velocity;
        public FloatTouchExtractorSettings Velocity => _velocity;

        [SerializeField] private FloatTouchExtractorSettings _pressure;
        public FloatTouchExtractorSettings Pressure => _pressure;
    }
}