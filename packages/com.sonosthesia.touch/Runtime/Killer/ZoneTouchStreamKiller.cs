using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ZoneTouchStreamKiller : TouchStreamKiller
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            TouchEventStreamContainer _eventStreamContainer = Extract<TouchEventStreamContainer>(other);

            if (_eventStreamContainer)
            {
                Kill(_eventStreamContainer);
            }
        }

        protected virtual TStream Extract<TStream>(Collider other) where TStream : TouchEventStreamContainer
        {
            return other.GetComponentInParent<TStream>();
        }
    }
}