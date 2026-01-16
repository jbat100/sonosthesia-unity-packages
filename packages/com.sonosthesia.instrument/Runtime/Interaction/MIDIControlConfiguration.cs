using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public interface IMIDIControlConfiguration<in TEvent> where TEvent : struct
    {
        IMIDIChannelExtractor<TEvent> Channel { get; }
        int ReferenceValue { get; }
        IInteractiveEnvelopeSettings<TEvent> Control { get; }       
    }

    public class MIDIControlConfiguration<TEvent, TChannelExtractor, TEnvelope> 
        : ScriptableObject, IMIDIControlConfiguration<TEvent>
        where TEvent : struct where TChannelExtractor : IMIDIChannelExtractor<TEvent> 
        where TEnvelope : IInteractiveEnvelopeSettings<TEvent>
    {
        [SerializeField] private TChannelExtractor _channel;
        public IMIDIChannelExtractor<TEvent> Channel => _channel;
        
        [SerializeField] private int _referenceValue;
        public int ReferenceValue => _referenceValue;

        [SerializeField] private TEnvelope _control;
        public IInteractiveEnvelopeSettings<TEvent> Control => _control;
    }
}