using System;
using Sonosthesia.Utils;
using UnityEngine.Events;

namespace Sonosthesia.Signal
{
    public class IntentSignalRelayReceiver : SignalRelayReceiver<Intent>
    {
        [Serializable]
        public class KeyEvent : UnityEvent<string>
        {
            
        }

        public KeyEvent onIntentKey;

        protected override void OnBroadcast(Intent value)
        {
            base.OnBroadcast(value);
            onIntentKey.Invoke(value.Key);
        }
    }
}