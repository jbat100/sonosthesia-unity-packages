using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    public class TouchMeshNoiseConfiguration : ScriptableObject
    {
        [SerializeField] private CatlikeNoiseType _noiseType = CatlikeNoiseType.Simplex; 
        public CatlikeNoiseType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _crossFadeType = EaseType.easeInOutSine;
        public EaseType CrossFadeType => _crossFadeType;

        [SerializeField] private bool _falloff;
        public bool Falloff => _falloff;
        
        [SerializeField] private EaseType _falloffType = EaseType.linear;
        public EaseType FalloffType => _falloffType;
        
        [SerializeField] private int _frequency;
        public int Frequency => _frequency;

        [SerializeField] private TouchEnvelopeSettings _radius;
        public TouchEnvelopeSettings Radius => _radius;
        
        [SerializeField] private TouchEnvelopeSettings _displacement;
        public TouchEnvelopeSettings Displacement => _displacement;
        
        [SerializeField] private TouchEnvelopeSettings _speed;
        public TouchEnvelopeSettings Speed => _speed;

    }
}