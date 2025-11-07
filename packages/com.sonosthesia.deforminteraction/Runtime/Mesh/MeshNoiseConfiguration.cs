using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    public interface IMeshNoiseConfiguration<in TEvent> where TEvent : struct, IInteractionEvent
    {
        CatlikeNoiseType NoiseType { get; }
        EaseType CrossFadeType { get; }
        int Frequency { get; }
        DynamicTrackingSettings ActorTracking { get; }
        SpatialFalloffSettings SpatialFalloff { get; }
        IInteractiveEnvelopeSettings<TEvent> Radius { get; }
        IInteractiveEnvelopeSettings<TEvent> Displacement { get; }
        IInteractiveEnvelopeSettings<TEvent> Speed { get; }
    }
    
    public class MeshNoiseConfiguration<TEvent, TEnvelope> : ScriptableObject, IMeshNoiseConfiguration<TEvent>
        where TEvent : struct, IInteractionEvent where TEnvelope : IInteractiveEnvelopeSettings<TEvent>
    {
        [SerializeField] private CatlikeNoiseType _noiseType = CatlikeNoiseType.Simplex; 
        public CatlikeNoiseType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _crossFadeType = EaseType.easeInOutSine;
        public EaseType CrossFadeType => _crossFadeType;

        [SerializeField] private int _frequency = 1;
        public int Frequency => _frequency;
        
        [SerializeField] private DynamicTrackingSettings _actorTracking;
        public DynamicTrackingSettings ActorTracking => _actorTracking;

        [SerializeField] private SpatialFalloffSettings _spatialFalloff;
        public SpatialFalloffSettings SpatialFalloff => _spatialFalloff;
        
        [SerializeField] private TEnvelope _radius;
        public IInteractiveEnvelopeSettings<TEvent> Radius => _radius;
        
        [SerializeField] private TEnvelope _displacement;
        public IInteractiveEnvelopeSettings<TEvent> Displacement => _displacement;
        
        [SerializeField] private TEnvelope _speed;
        public IInteractiveEnvelopeSettings<TEvent> Speed => _speed;
    }
}