using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class SignalGroup<T> : MonoBehaviour where T : struct
    {
        private Dictionary<string, Signal<T>> _signals;

        public Signal<T> GetSignal(string key)
        {
            if (_signals != null)
            {
                return _signals[key];
            }
            _signals = new Dictionary<string, Signal<T>>();
            foreach (Transform child in transform)
            {
                Signal<T> signal = child.GetComponent<Signal<T>>();
                if (!signal)
                {
                    continue;
                }
                if (_signals.ContainsKey(child.name))
                {
                    Debug.LogError($"Unexpected multiple signals for key {child.name}");
                    continue;
                }
                _signals[child.name] = signal;
            }
            return _signals[key];
        }
    }    
}


