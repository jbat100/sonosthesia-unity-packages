using System;
using System.Collections.Generic;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Application
{
    // Note : could be done by combining Signals, Adaptors, and NamedFieldTarget but practicality is a big factor here
    
    public class ImageSpriteSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Image> _images;
        
        [SerializeField] private BoolSignalRelay _switch;

        [SerializeField] private Sprite _on;

        [SerializeField] private Sprite _off;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_switch)
            {
                _switch.Observable.DistinctUntilChanged().Subscribe(on =>
                {
                    Sprite sprite = on ? _on : _off;
                    if (!sprite)
                    {
                        return;
                    }
                    foreach (Image image in _images)
                    {
                        image.sprite = sprite;
                    }
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}