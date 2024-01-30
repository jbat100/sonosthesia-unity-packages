using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ZoneTriggerStreamKiller : TriggerStreamKiller
    {
        public enum StreamType
        {
            None,
            Actor,
            Source
        }

        [SerializeField] private StreamType _streamType;
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerStream stream = _streamType switch
            {
                StreamType.Actor => Extract<BaseTriggerActor>(other),
                StreamType.Source => Extract<BaseTriggerSource>(other),
                _ => null
            };

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