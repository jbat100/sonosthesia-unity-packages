using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class BuilderTrigger : Trigger
    {
        [SerializeField] private EnvelopeBuilder _envelopeBuilder;

        protected override IEnvelope DefaultEnvelope => _envelopeBuilder ? _envelopeBuilder.Build() : base.DefaultEnvelope;
    }
}