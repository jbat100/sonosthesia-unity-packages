using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchPeakConfiguration", menuName = "Sonosthesia/Touch/TouchPeakConfiguration")]
    public class TouchPeakConfiguration: ScriptableObject
    {
        [SerializeField] [Range(0, 1)] private float _randomization;
        public float Randomization => _randomization;
        
        [SerializeField] private TouchEnvelopeSettings _magnitude;
        public TouchEnvelopeSettings Magnitude => _magnitude;

        [SerializeField] private TouchEnvelopeSettings _duration;
        public TouchEnvelopeSettings Duration => _duration;
    }
}