using UnityEngine;

namespace Sonosthesia.Audio
{
    [CreateAssetMenu(fileName = "AudioPeakDetector", menuName = "Sonosthesia/Settings/AudioPeakDetector")]
    public class AudioPeakDetectorSettings : ScriptableObject
    {
        [SerializeField] private float _magnitudeThreshold = .1f;
        public float MagnitudeThreshold => _magnitudeThreshold;
        
        [SerializeField] private float _maximumDuration = .1f;
        public float MaximumDuration => _maximumDuration;
        
    }
}