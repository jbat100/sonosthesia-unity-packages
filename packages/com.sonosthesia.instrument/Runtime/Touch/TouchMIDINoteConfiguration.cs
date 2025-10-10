using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "TouchMIDINoteConfiguration", menuName = "Sonosthesia/Instrument/TouchMIDINoteConfiguration")]
    public class TouchMIDINoteConfiguration : ScriptableObject
    {
        [SerializeField] private bool _applyPressure;
        public bool ApplyPressure => _applyPressure;
        
        [SerializeField] private MIDIChannelTouchExtractorSettings _channel;
        public MIDIChannelTouchExtractorSettings Channel => _channel;

        [SerializeField] private MIDIPitchTouchExtractorSettings _pitch;
        public MIDIPitchTouchExtractorSettings Pitch => _pitch;

        [SerializeField] private FloatTouchDynamicExtractorSettings _velocity;
        public FloatTouchDynamicExtractorSettings Velocity => _velocity;

        [SerializeField] private FloatTouchDynamicExtractorSettings _pressure;
        public FloatTouchDynamicExtractorSettings Pressure => _pressure;
    }
}