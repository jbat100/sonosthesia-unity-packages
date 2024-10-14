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
        [SerializeField] private float _speed = 1f;
        public float Speed => _speed;
        
        [SerializeField] private Noise4DType _noiseType = Noise4DType.Simplex; 
        public Noise4DType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _falloffType = EaseType.linear;
        public EaseType FalloffType => _falloffType;
        
        [SerializeField] private float _frequency = 5;
        public float Frequency => _frequency;
        
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