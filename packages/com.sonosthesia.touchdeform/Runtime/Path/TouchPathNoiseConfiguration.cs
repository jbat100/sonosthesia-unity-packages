using UnityEngine;
using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Touch;

namespace Sonosthesia.TouchDeform
{
    [CreateAssetMenu(fileName = "TouchPathNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchPathNoiseConfiguration")]
    public class TouchPathNoiseConfiguration : ScriptableObject
    {
        [Header("Noise")]
        
        [SerializeField] private bool _trackPosition;
        public bool TrackPosition => _trackPosition;

        [SerializeField] private Noise4DType _noiseType = Noise4DType.Simplex;
        public Noise4DType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _falloffType = EaseType.linear;
        public EaseType FalloffType => _falloffType;
        
        [Header("Touch")]
        
        [SerializeField] private TouchEnvelopeSettings _radius;
        public TouchEnvelopeSettings Radius => _radius;

        [SerializeField] private TouchEnvelopeSettings _displacement;
        public TouchEnvelopeSettings Displacement => _displacement;

        [SerializeField] private TouchEnvelopeSettings _frequency;
        public TouchEnvelopeSettings Frequency => _frequency;
        
        [SerializeField] private TouchEnvelopeSettings _speed;
        public TouchEnvelopeSettings Speed => _speed;
    }
}