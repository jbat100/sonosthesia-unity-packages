using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFade : MonoBehaviour
    {
        [SerializeField] private FloatSignalRelay _fade;
        
        private CanvasGroup _canvasGroup;
        private IDisposable _subscription;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_fade && _canvasGroup)
            {
                _fade.Observable.Subscribe(f =>
                {
                    _canvasGroup.alpha = 1f - f;
                });
            }
        }
    }
}