using UnityEngine;

namespace Sonosthesia.Envelope
{
    [CreateAssetMenu(fileName = "PhasedEnvelopeFactory", menuName = "Sonosthesia/Envelope/PhasedEnvelopeFactory")]
    public class PhasedEnvelopeFactory : EnvelopeFactory
    {
        [SerializeField] private PhasedEnvelopeSettings _settings;

        public override IEnvelope Build() => _settings.Build();
    }
}