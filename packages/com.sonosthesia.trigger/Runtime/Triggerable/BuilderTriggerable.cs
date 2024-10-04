using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class BuilderTriggerable : Triggerable
    {
        [SerializeField] private EnvelopeBuilder _envelopeBuilder;

        protected override IEnvelope DefaultEnvelope => _envelopeBuilder ? _envelopeBuilder.Build() : base.DefaultEnvelope;
    }
}