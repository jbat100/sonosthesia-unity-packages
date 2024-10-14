using System;
using UnityEngine;

namespace Sonosthesia.Envelope
{
    [Obsolete("Use Envelope Factories")]
    public class AnimationCurveEnvelopeBuilder : EnvelopeBuilder
    {
        [SerializeField] private AnimationCurve _animationCurve;

        [SerializeField] private float _timeScale = 1f;

        public override IEnvelope Build()
        {
            return new WarpedEnvelope(new AnimationCurveEnvelope(_animationCurve), 1f, _timeScale);
        }
    }
}