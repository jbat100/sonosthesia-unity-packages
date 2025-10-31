using System;
using Sonosthesia.Envelope;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    [Obsolete("Use EnvelopeSettings")]
    public class BuilderTrigger : Trigger
    {
        [SerializeField] private EnvelopeBuilder _envelopeBuilder;
    }
}