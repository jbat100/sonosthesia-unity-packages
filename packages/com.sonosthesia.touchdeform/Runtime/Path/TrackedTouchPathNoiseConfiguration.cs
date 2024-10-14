using UnityEngine;
using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Touch;

namespace Sonosthesia.TouchDeform
{
    
    [CreateAssetMenu(fileName = "TrackedTouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TrackedTouchPathNoiseConfiguration")]
    public class TrackedTouchPathNoiseConfiguration : ScriptableObject
    {
        [SerializeField] private float _speed;
        public float Speed => _speed = 1f;
        
        [SerializeField] private bool _trackPosition;
        public bool TrackPosition => _trackPosition;

        [SerializeField] private Noise4DType _noiseType;
        public Noise4DType NoiseType => _noiseType;
        
        [SerializeField] private float _frequency;
        public float Frequency => _frequency;
        
        [SerializeField] private EaseType _falloffType;
        public EaseType FalloffType => _falloffType;
        
        [SerializeField] private bool _trackRadius;
        public bool TrackRadius => _trackRadius;
        
        [SerializeField] private FloatTouchExtractorSettings _radiusExtractor;
        public FloatTouchExtractorSettings RadiusExtractor => _radiusExtractor;

        [SerializeField] private bool _trackDisplacement;
        public bool TrackDisplacement => _trackDisplacement;

        [SerializeField] private FloatTouchExtractorSettings _displacementValueExtractor;
        public FloatTouchExtractorSettings DisplacementValueExtractor => _displacementValueExtractor;

        [SerializeField] private FloatTouchExtractorSettings _displacementTimeExtractor;
        public FloatTouchExtractorSettings DisplacementTimeExtractor => _displacementTimeExtractor;

        [SerializeField] private EnvelopeSettings _displacementEnvelope;
        public EnvelopeSettings DisplacementEnvelope => _displacementEnvelope;

        [SerializeField] private FloatTouchExtractorSettings _displacementReleaseExtractor;
        public FloatTouchExtractorSettings DisplacementReleaseExtractor => _displacementReleaseExtractor;

        [SerializeField] private EaseType _displacementReleaseEaseType;
        public EaseType DisplacementReleaseEaseType => _displacementReleaseEaseType;
    }
}