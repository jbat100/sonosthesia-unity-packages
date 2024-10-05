using UnityEngine;

namespace Sonosthesia.Envelope
{
    [CreateAssetMenu(fileName = "AnimationCurveEnvelopeFactory", menuName = "Sonosthesia/Envelope/AnimationCurveEnvelopeFactory")]
    public class AnimationCurveEnvelopeFactory : EnvelopeFactory
    {
        [SerializeField] private AnimationCurve _animationCurve;
        
        public override IEnvelope Build() => new AnimationCurveEnvelope(_animationCurve);
    }
}