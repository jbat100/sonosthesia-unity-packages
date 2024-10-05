using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ZoneTouchStreamKiller : TouchStreamKiller
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            TouchStream stream = Extract<TouchEndpoint>(other);

            if (stream)
            {
                Kill(stream);
            }
        }

        private TStream Extract<TStream>(Collider other) where TStream : TouchStream
        {
            return other.GetComponentInParent<TStream>();
        }
    }
}