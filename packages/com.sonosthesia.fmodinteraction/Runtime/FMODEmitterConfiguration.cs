using FMODUnity;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.FMODInteraction
{
    public interface IFMODEmitterConfiguration<in TEvent> where TEvent : struct, IInteractionEvent
    {
        PrefabSelectorSettings<StudioEventEmitter> Emitter { get; }
        DynamicTrackingSettings PositionTracking { get; }
        IInteractiveEnvelopeSettings<TEvent> Volume { get; }
        IInteractiveEnvelopeSettings<TEvent> Excitation { get; }
        IInteractiveEnvelopeSettings<TEvent> Body { get; }
    }

    public class FMODEmitterConfiguration<TEvent, TEnvelope> : ScriptableObject, IFMODEmitterConfiguration<TEvent>
        where TEvent : struct, IInteractionEvent where TEnvelope : IInteractiveEnvelopeSettings<TEvent>
    {
        [SerializeField] private PrefabSelectorSettings<StudioEventEmitter> _emitter;
        public PrefabSelectorSettings<StudioEventEmitter> Emitter => _emitter;
        
        [SerializeField] private DynamicTrackingSettings _positionTracking;
        public DynamicTrackingSettings PositionTracking => _positionTracking;

        [SerializeField] private TEnvelope _volume;
        public IInteractiveEnvelopeSettings<TEvent> Volume => _volume;
        
        [SerializeField] private TEnvelope _excitation;
        public IInteractiveEnvelopeSettings<TEvent> Excitation => _excitation;
        
        [SerializeField] private TEnvelope _body;
        public IInteractiveEnvelopeSettings<TEvent> Body => _body;
    }
}