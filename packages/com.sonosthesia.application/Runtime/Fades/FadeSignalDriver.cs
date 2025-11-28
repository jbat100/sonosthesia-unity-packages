using System;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Application
{
    public class FadeSignalDriver : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<ISignal<float>> _source;
        
        [SerializeField] private AbstractFade _fade;

        private IDisposable _subscription;
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source.Value != null)
            {
                _subscription = _source.Value.Observable.Subscribe(value =>
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