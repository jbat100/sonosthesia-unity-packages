using System;
using UnityEngine;
using Sonosthesia.Signal;
using UniRx;

namespace Sonosthesia.Application
{
    public class FadeSignalDriver : MonoBehaviour
    {
        [SerializeField] private Signal<float> _source;
        
        [SerializeField] private AbstractFade _fade;

        private IDisposable _subscription;
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.SignalObservable.Subscribe(value =>
                {
                    if (!_fade)
                    {
                        return;
                    }

                    _fade.Fade = value;
                });
            }
        }
    }
}