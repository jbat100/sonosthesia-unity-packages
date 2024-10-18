using UnityEngine;
using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Touch;

namespace Sonosthesia.TouchDeform
{
    [CreateAssetMenu(fileName = "TrackedTouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TrackedTouchPathNoiseConfiguration")]
    public class TrackedTouchPathNoiseConfiguration : ScriptableObject
    {
        [Header("Noise")]
        
        [SerializeField] private bool _trackPosition;
        public bool TrackPosition => _trackPosition;

        [SerializeField] private Noise4DType _noiseType = Noise4DType.Simplex;
        public Noise4DType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _falloffType = EaseType.linear;
        public EaseType FalloffType => _falloffType;
        
        [Header("Touch")]
        
        [SerializeField] private TrackedTouchEnvelopeSettings _radius;
        public TrackedTouchEnvelopeSettings Radius => _radius;

        [SerializeField] private TrackedTouchEnvelopeSettings _displacement;
        public TrackedTouchEnvelopeSettings Displacement => _displacement;

        [SerializeField] private TrackedTouchEnvelopeSettings _frequency;
        public TrackedTouchEnvelopeSettings Frequency => _frequency;
        
        [SerializeField] private TrackedTouchEnvelopeSettings _speed;
        public TrackedTouchEnvelopeSettings Speed => _speed;
    }
}