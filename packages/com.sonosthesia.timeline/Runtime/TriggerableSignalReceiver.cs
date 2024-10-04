using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sonosthesia.Trigger;

namespace Sonosthesia.Timeline
{
    public class TriggerableSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public SignalAssetEventInfo[] signalAssetEvents;

        [Serializable]
        public class SignalAssetEventInfo
        {
            public SignalAsset signalAsset;
            public ParameterizedEvent events;
            public BuilderTriggerable.Payload payload;

            [Serializable]
            public class ParameterizedEvent : UnityEvent<BuilderTriggerable.Payload> { }
        }
        
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            SignalEmitter signal = notification as SignalEmitter;
            Debug.Log($"{this} received signal {signal}");
            if (signal != null && signal.asset != null)
            {
                Debug.Log($"{this} received signal {signal} sending to {signalAssetEvents.Length} listeners");
                foreach (SignalAssetEventInfo info in signalAssetEvents)
                {
                    if (info.signalAsset == signal.asset)
                    {
                        info.events?.Invoke(info.payload);
                    }
                }
            }
        }
    }
}