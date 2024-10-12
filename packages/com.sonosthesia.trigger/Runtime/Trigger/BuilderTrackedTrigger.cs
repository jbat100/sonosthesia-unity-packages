using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class BuilderTrackedTrigger : TrackedTrigger
    {
        [SerializeField] private EnvelopeBuilder _startEnvelopeBuilder;
        
        [SerializeField] private EnvelopeBuilder _endEnvelopeBuilder;

        protected override IEnvelope DefaultStartEnvelope 
            => _startEnvelopeBuilder ? _startEnvelopeBuilder.Build() : base.DefaultStartEnvelope;

        protected override IEnvelope DefaultEndEnvelope
            => _endEnvelopeBuilder ? _endEnvelopeBuilder.Build() : base.DefaultEndEnvelope;
    }
}