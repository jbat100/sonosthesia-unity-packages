using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    [CreateAssetMenu(fileName = "TouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchPathNoiseConfiguration")]

    public class TouchPathNoiseConfiguration : ScriptableObject
    {
        [SerializeField] private float _speed;
        public float Speed => _speed = 1f;
        
        [SerializeField] private Noise4DType _noiseType;
        public Noise4DType NoiseType => _noiseType = Noise4DType.Simplex;
        
        [SerializeField] private EaseType _falloffType;
        public EaseType FalloffType => _falloffType = EaseType.linear;
        
        [SerializeField] private float _frequency;
        public float Frequency => _frequency = 5;
        
        [SerializeField] private FloatTouchExtractorSettings _radiusExtractor;
        public FloatTouchExtractorSettings RadiusExtractor => _radiusExtractor;

        [SerializeField] private FloatTouchExtractorSettings _displacementValueExtractor;
        public FloatTouchExtractorSettings DisplacementValueExtractor => _displacementValueExtractor;

        [SerializeField] private FloatTouchExtractorSettings _displacementTimeExtractor;
        public FloatTouchExtractorSettings DisplacementTimeExtractor => _displacementTimeExtractor;

        [SerializeField] private EnvelopeSettings _displacementEnvelope;
        public EnvelopeSettings DisplacementEnvelope => _displacementEnvelope;
    }
}