using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [CreateAssetMenu(fileName = "PeakDetector", menuName = "Sonosthesia/Settings/PeakDetector")]
    public class PeakDetectorSettings : ScriptableObject
    {
        [SerializeField] private float _magnitudeThreshold = .1f;
        public float MagnitudeThreshold => _magnitudeThreshold;
        
        [SerializeField] private float _maximumDuration = .1f;
        public float MaximumDuration => _maximumDuration;

        [SerializeField] private FloatProcessor _valuePostProcessor;
        public FloatProcessor ValuePostProcessor => _valuePostProcessor;

    }
}