using UnityEngine;

namespace Sonosthesia.Envelope
{
    [CreateAssetMenu(fileName = "EnvelopeFactory", menuName = "Sonosthesia/Envelope/EnvelopeFactory")]
    public class EnvelopeFactory : AbstractEnvelopeFactory
    {
        [SerializeField] private EnvelopeSettings _settings;

        public override IEnvelope Build()
        {
            return _settings?.Build();
        }
    }
}