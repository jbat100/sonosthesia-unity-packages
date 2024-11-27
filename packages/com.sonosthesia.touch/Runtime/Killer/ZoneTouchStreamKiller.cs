using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ZoneTouchStreamKiller : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            TouchEventChannel eventChannel = Extract<TouchEventChannel>(other);

            if (eventChannel)
            {
                eventChannel.KillAllStreams();
            }
        }

        protected virtual TStream Extract<TStream>(Collider other) where TStream : TouchEventChannel
        {
            return other.GetComponentInParent<TStream>();
        }
    }
}