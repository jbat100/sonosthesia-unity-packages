using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ZoneTriggerStreamKiller : TriggerStreamKiller
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerStream stream = Extract<TriggerEndpoint>(other);

            if (stream)
            {
                Kill(stream);
            }
        }

        private TStream Extract<TStream>(Collider other) where TStream : TriggerStream
        {
            return other.GetComponentInParent<TStream>();
        }
    }
}