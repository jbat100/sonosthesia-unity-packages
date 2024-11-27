using System;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    [Obsolete("Use Envelope Factories")]
    public class PhasedEnvelopeBuilder : EnvelopeBuilder
    {
        [SerializeField] private EnvelopeSettings _settings;

        public override IEnvelope Build() => _settings.Build();
    }
}