using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    [CreateAssetMenu(fileName = "TouchMeshNoiseConfiguration", menuName = "Sonosthesia/Touch/TouchMeshNoiseConfiguration")]
    public class TouchMeshNoiseConfiguration : ScriptableObject
    {
        [SerializeField] private CatlikeNoiseType _noiseType = CatlikeNoiseType.Simplex; 
        public CatlikeNoiseType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _crossFadeType = EaseType.easeInOutSine;
        public EaseType CrossFadeType => _crossFadeType;

        [SerializeField] private int _frequency = 1;
        public int Frequency => _frequency;
        
        [SerializeField] private DynamicTrackingSettings _actorTracking;
        public DynamicTrackingSettings ActorTracking => _actorTracking;

        [SerializeField] private TouchSpatialFalloffSettings _spatialFalloff;
        public TouchSpatialFalloffSettings SpatialFalloff => _spatialFalloff;
        
        [SerializeField] private TouchEnvelopeSettings _radius;
        public TouchEnvelopeSettings Radius => _radius;
        
        [SerializeField] private TouchEnvelopeSettings _displacement;
        public TouchEnvelopeSettings Displacement => _displacement;
        
        [SerializeField] private TouchEnvelopeSettings _speed;
        public TouchEnvelopeSettings Speed => _speed;
    }
}