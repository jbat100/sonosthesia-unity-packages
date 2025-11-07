using Sonosthesia.Deform;
using Sonosthesia.Ease;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    public interface IPathNoiseConfiguration<in TEvent> where TEvent : struct, IInteractionEvent
    {
        bool TrackPosition { get; }
        Noise4DType NoiseType { get; }
        EaseType FalloffType { get; }
        IInteractiveEnvelopeSettings<TEvent> Radius { get; }
        IInteractiveEnvelopeSettings<TEvent> Displacement { get; }
        IInteractiveEnvelopeSettings<TEvent> Frequency { get; }
        IInteractiveEnvelopeSettings<TEvent> Speed { get; }
    }
    
    public class PathNoiseConfiguration<TEvent, TEnvelope> : ScriptableObject, IPathNoiseConfiguration<TEvent>
        where TEvent : struct, IInteractionEvent where TEnvelope : IInteractiveEnvelopeSettings<TEvent>
    {
        [SerializeField] private bool _trackPosition;
        public bool TrackPosition => _trackPosition;

        [SerializeField] private Noise4DType _noiseType = Noise4DType.Simplex;
        public Noise4DType NoiseType => _noiseType;
        
        [SerializeField] private EaseType _falloffType = EaseType.linear;
        public EaseType FalloffType => _falloffType;

        [SerializeField] private TEnvelope _radius;
        public IInteractiveEnvelopeSettings<TEvent> Radius => _radius;

        [SerializeField] private TEnvelope _displacement;
        public IInteractiveEnvelopeSettings<TEvent> Displacement => _displacement;

        [SerializeField] private TEnvelope _frequency;
        public IInteractiveEnvelopeSettings<TEvent> Frequency => _frequency;
        
        [SerializeField] private TEnvelope _speed;
        public IInteractiveEnvelopeSettings<TEvent> Speed => _speed;
    }

}