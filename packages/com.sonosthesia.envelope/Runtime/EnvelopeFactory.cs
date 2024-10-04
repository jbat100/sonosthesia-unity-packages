using UnityEngine;

namespace Sonosthesia.Envelope
{
    public abstract class EnvelopeFactory : ScriptableObject
    {
        public abstract IEnvelope Build();
    }
}