using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    public abstract class BaseAudioController : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intents;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_intents)
            {
                _subscription = _intents.Observable.Subscribe(i => Play(i.Key));
            }
        }
        
        public abstract void Play(string eventName);
        
        
    }
}