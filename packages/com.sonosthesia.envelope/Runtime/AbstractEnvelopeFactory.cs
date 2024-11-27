using UnityEngine;

namespace Sonosthesia.Envelope
{
    public abstract class AbstractEnvelopeFactory : ScriptableObject
    {
        public abstract IEnvelope Build();
    }
}